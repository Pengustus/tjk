using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tjk.Data;
using tjk.Models;
using tjk.Services;

namespace tjk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, IProductService productService, AppDbContext dbContext)
        {
            _logger = logger;
            _productService = productService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Индустриални машини за арматурен двор и префaбрициран бетон";
            ViewBag.Categories = _productService.GetCategories();
            ViewBag.FeaturedProducts = _productService.GetProducts(featuredOnly: true);
            return View();
        }

        [Route("products")]
        [Route("products/{category?}")]
        public IActionResult Products(string? category)
        {
            var categories = _productService.GetCategories();
            var products = _productService.GetProducts(categorySlug: category);
            
            var selectedCategory = categories.FirstOrDefault(c => c.Slug.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            ViewData["Title"] = selectedCategory != null ? selectedCategory.Name : "Каталог Машини";
            ViewBag.Categories = categories;
            ViewBag.SelectedCategorySlug = category;
            ViewBag.SelectedCategoryName = selectedCategory?.Name ?? "Всички категории";

            return View(products);
        }

        [Route("product/{slug}")]
        public IActionResult ProductDetail(string slug)
        {
            var product = _productService.GetProductBySlug(slug);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Title"] = product.Name;
            ViewBag.RelatedProducts = _productService.GetProducts(categorySlug: product.CategoryId)
                .Where(p => p.Id != product.Id)
                .Take(3)
                .ToList();

            return View(product);
        }

        [Route("services")]
        public IActionResult Services()
        {
            ViewData["Title"] = "Услуги и Техническа поддръжка";
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            ViewData["Title"] = "За TJK Delta";
            return View();
        }

        [Route("contact")]
        [HttpGet]
        public IActionResult Contact(string? product)
        {
            ViewData["Title"] = "Контакти";
            var model = new ContactFormViewModel
            {
                SelectedProduct = product ?? ""
            };
            return View(model);
        }

        [Route("contact")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            ViewData["Title"] = "Контакти";
            if (ModelState.IsValid)
            {
                try
                {
                    var inquiry = new Inquiry
                    {
                        Name = model.Name,
                        Company = model.Company,
                        Email = model.Email,
                        Phone = model.Phone,
                        SelectedProduct = model.SelectedProduct,
                        Message = model.Message,
                        CreatedAt = DateTime.UtcNow,
                        Status = "Ново",
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                    };

                    _dbContext.Inquiries.Add(inquiry);
                    await _dbContext.SaveChangesAsync();

                    model.SubmittedSuccessfully = true;
                    _logger.LogInformation("Contact request #{Id} submitted by {Name} ({Email}) for product '{Product}'",
                        inquiry.Id, model.Name, model.Email, model.SelectedProduct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save inquiry to database for {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Възникна временна грешка при обработката на запитването. Моля, опитайте отново или се свържете с нас директно по телефон.");
                }
            }
            return View(model);
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Политика за поверителност";
            return View();
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Sitemap()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var categories = _productService.GetCategories();
            var products = _productService.GetProducts();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Static pages
            string[] staticUrls = { "", "products", "services", "about", "contact", "privacy" };
            foreach (var rel in staticUrls)
            {
                var loc = string.IsNullOrEmpty(rel) ? baseUrl : $"{baseUrl}/{rel}";
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{loc}</loc>");
                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine($"    <priority>{(string.IsNullOrEmpty(rel) ? "1.0" : "0.8")}</priority>");
                sb.AppendLine("  </url>");
            }

            // Categories
            foreach (var cat in categories)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}/products/{cat.Slug}</loc>");
                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine("    <priority>0.8</priority>");
                sb.AppendLine("  </url>");
            }

            // Products
            foreach (var prod in products)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}/product/{prod.Slug}</loc>");
                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine("    <priority>0.9</priority>");
                sb.AppendLine("  </url>");
            }

            sb.AppendLine("</urlset>");
            return Content(sb.ToString(), "application/xml", System.Text.Encoding.UTF8);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
