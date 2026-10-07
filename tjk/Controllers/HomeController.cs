using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tjk.Models;
using tjk.Services;

namespace tjk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
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
        public IActionResult Contact(ContactFormViewModel model)
        {
            ViewData["Title"] = "Контакти";
            if (ModelState.IsValid)
            {
                model.SubmittedSuccessfully = true;
                _logger.LogInformation("Contact request submitted by {Name} ({Email}) for product '{Product}'", model.Name, model.Email, model.SelectedProduct);
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Политика за поверителност";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
