using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tjk.Data;
using tjk.Models;
using tjk.Services;

namespace tjk.Controllers
{
    [Authorize]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductService _productService;
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            AppDbContext dbContext,
            IProductService productService,
            ILogger<AdminController> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _productService = productService;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        #region Authentication (Login / Logout)

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Inquiries));
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var expectedUser = _configuration["AdminSettings:Username"] ?? "admin";
            var expectedPass = _configuration["AdminSettings:Password"] ?? "SlavaNaXi1953?";

            if (string.Equals(model.Username, expectedUser, StringComparison.OrdinalIgnoreCase) &&
                model.Password == expectedPass)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Admin user {User} logged in successfully.", model.Username);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction(nameof(Inquiries));
            }

            ModelState.AddModelError(string.Empty, "Грешно потребителско име или парола.");
            return View(model);
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Admin logged out.");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Inquiries (CRM)

        [HttpGet("")]
        [HttpGet("inquiries")]
        public async Task<IActionResult> Inquiries(string? status)
        {
            ViewData["Title"] = "Запитвания за оферти (CRM / Администрация)";
            var query = _dbContext.Inquiries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(i => i.Status == status);
            }

            var inquiries = await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            ViewBag.TotalCount = await _dbContext.Inquiries.CountAsync();
            ViewBag.NewCount = await _dbContext.Inquiries.CountAsync(i => i.Status == "Ново");
            ViewBag.SelectedStatus = status;

            return View(inquiries);
        }

        [HttpPost("inquiries/{id}/status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, [FromForm] string status)
        {
            var inquiry = await _dbContext.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                inquiry.Status = status;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Inquiry #{Id} status updated to {Status}", id, status);
            }
            return RedirectToAction(nameof(Inquiries));
        }

        [HttpPost("inquiries/{id}/notes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(int id, [FromForm] string? notes)
        {
            var inquiry = await _dbContext.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                inquiry.AdminNotes = notes;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Inquiry #{Id} notes updated", id);
                TempData["SuccessMessage"] = "Бележката към запитването беше запазена успешно!";
            }
            return RedirectToAction(nameof(Inquiries));
        }

        [HttpPost("inquiries/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var inquiry = await _dbContext.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                _dbContext.Inquiries.Remove(inquiry);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Inquiry #{Id} deleted", id);
            }
            return RedirectToAction(nameof(Inquiries));
        }

        #endregion

        #region Machines & Offers Management (CRUD)

        [HttpGet("machines")]
        public async Task<IActionResult> Machines(string? category)
        {
            ViewData["Title"] = "Управление на машини & Оферти";
            var categories = _productService.GetCategories();
            var productsQuery = _dbContext.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == category);
            }

            var products = await productsQuery.ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.TotalMachines = await _dbContext.Products.CountAsync();
            ViewBag.FeaturedMachines = await _dbContext.Products.CountAsync(p => p.IsFeatured);

            return View(products);
        }

        [HttpGet("machines/create")]
        public IActionResult CreateMachine()
        {
            ViewData["Title"] = "Добавяне на нова машина / Оферта";
            ViewBag.Categories = _productService.GetCategories();

            var model = new MachineFormViewModel
            {
                IsFeatured = false,
                ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153800/Estribadora-WG12-1A-660x440.jpg",
                FeaturesText = "Автоматизирано CNC управление\nСерво задвижване с висока точност\nМинимален технологичен отпадък",
                SpecificationsText = "Диаметър: Ø6 - 16 mm\nСкорост: 100 м/мин\nМощност: 25 kW\nТочност: ±1 mm"
            };

            return View("MachineForm", model);
        }

        [HttpPost("machines/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMachine(MachineFormViewModel model)
        {
            ViewData["Title"] = "Добавяне на нова машина / Оферта";
            ViewBag.Categories = _productService.GetCategories();

            if (!ModelState.IsValid)
            {
                return View("MachineForm", model);
            }

            // Generate clean ID and Slug if not provided
            string slug = string.IsNullOrWhiteSpace(model.Slug) ? GenerateSlug(model.Name) : GenerateSlug(model.Slug);
            string id = !string.IsNullOrWhiteSpace(model.Id) ? model.Id.Trim().ToLower() : slug;

            // Ensure unique ID
            if (await _dbContext.Products.AnyAsync(p => p.Id == id))
            {
                id = $"{id}-{DateTime.UtcNow.Ticks % 10000}";
            }

            var category = await _dbContext.Categories.FindAsync(model.CategoryId);
            var finalImageUrl = await ProcessImageAsync(model.ImageUpload, model.ImageUrl);

            var product = new ProductItem
            {
                Id = id,
                Name = model.Name.Trim(),
                Slug = slug,
                CategoryId = model.CategoryId,
                CategoryName = category?.Name ?? "Общо оборудване",
                ShortDescription = model.ShortDescription.Trim(),
                FullDescription = model.FullDescription.Trim(),
                ImageUrl = finalImageUrl,
                Badge = string.IsNullOrWhiteSpace(model.Badge) ? null : model.Badge.Trim(),
                IsFeatured = model.IsFeatured,
                Features = ParseLines(model.FeaturesText),
                Specifications = ParseKeyValueLines(model.SpecificationsText)
            };

            _productService.CreateProduct(product);
            _logger.LogInformation("Machine '{Name}' (#{Id}) created by admin", product.Name, product.Id);

            TempData["SuccessMessage"] = $"Машината '{product.Name}' беше добавена успешно!";
            return RedirectToAction(nameof(Machines));
        }

        [HttpGet("machines/edit/{id}")]
        public async Task<IActionResult> EditMachine(string id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Title"] = $"Редакция на {product.Name}";
            ViewBag.Categories = _productService.GetCategories();

            var model = new MachineFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                CategoryId = product.CategoryId,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                ImageUrl = product.ImageUrl,
                Badge = product.Badge,
                IsFeatured = product.IsFeatured,
                FeaturesText = string.Join("\n", product.Features),
                SpecificationsText = string.Join("\n", product.Specifications.Select(s => $"{s.Key}: {s.Value}"))
            };

            return View("MachineForm", model);
        }

        [HttpPost("machines/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMachine(string id, MachineFormViewModel model)
        {
            ViewData["Title"] = $"Редакция на {model.Name}";
            ViewBag.Categories = _productService.GetCategories();

            if (!ModelState.IsValid)
            {
                return View("MachineForm", model);
            }

            var existing = _productService.GetProductById(id);
            if (existing == null)
            {
                return NotFound();
            }

            string slug = string.IsNullOrWhiteSpace(model.Slug) ? GenerateSlug(model.Name) : GenerateSlug(model.Slug);
            var category = await _dbContext.Categories.FindAsync(model.CategoryId);
            var finalImageUrl = await ProcessImageAsync(model.ImageUpload, model.ImageUrl);

            existing.Name = model.Name.Trim();
            existing.Slug = slug;
            existing.CategoryId = model.CategoryId;
            existing.CategoryName = category?.Name ?? existing.CategoryName;
            existing.ShortDescription = model.ShortDescription.Trim();
            existing.FullDescription = model.FullDescription.Trim();
            existing.ImageUrl = finalImageUrl;
            existing.Badge = string.IsNullOrWhiteSpace(model.Badge) ? null : model.Badge.Trim();
            existing.IsFeatured = model.IsFeatured;
            existing.Features = ParseLines(model.FeaturesText);
            existing.Specifications = ParseKeyValueLines(model.SpecificationsText);

            _productService.UpdateProduct(existing);
            _logger.LogInformation("Machine '{Name}' (#{Id}) updated by admin", existing.Name, existing.Id);

            TempData["SuccessMessage"] = $"Машината '{existing.Name}' беше актуализирана успешно!";
            return RedirectToAction(nameof(Machines));
        }

        [HttpPost("machines/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMachine(string id)
        {
            var product = _productService.GetProductById(id);
            if (product != null)
            {
                _productService.DeleteProduct(id);
                _logger.LogInformation("Machine #{Id} deleted by admin", id);
                TempData["SuccessMessage"] = $"Машината беше изтрита успешно!";
            }
            return RedirectToAction(nameof(Machines));
        }

        #endregion

        #region Helpers

        private async Task<string> ProcessImageAsync(IFormFile? file, string? fallbackUrl)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "data", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var safeFileName = $"machine-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 8)}{extension}";
                var filePath = Path.Combine(uploadsFolder, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/data/uploads/{safeFileName}";
            }

            return !string.IsNullOrWhiteSpace(fallbackUrl) ? fallbackUrl.Trim() : "/images/logo.png";
        }

        private static List<string> ParseLines(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();
        }

        private static Dictionary<string, string> ParseKeyValueLines(string? text)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(text)) return dict;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                int colonIdx = trimmed.IndexOf(':');
                if (colonIdx > 0)
                {
                    string key = trimmed.Substring(0, colonIdx).Trim();
                    string val = trimmed.Substring(colonIdx + 1).Trim();
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
                    {
                        dict[key] = val;
                    }
                }
                else
                {
                    dict[trimmed] = "";
                }
            }

            return dict;
        }

        private static string GenerateSlug(string text)
        {
            string slug = text.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9а-я\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
            return string.IsNullOrEmpty(slug) ? "machine-" + DateTime.UtcNow.Ticks % 10000 : slug;
        }

        #endregion
    }
}