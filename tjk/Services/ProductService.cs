using Microsoft.EntityFrameworkCore;
using tjk.Data;
using tjk.Models;

namespace tjk.Services
{
    public interface IProductService
    {
        List<CategoryItem> GetCategories();
        List<ProductItem> GetProducts(string? categorySlug = null, bool? featuredOnly = null);
        ProductItem? GetProductBySlug(string slug);
        ProductItem? GetProductById(string id);
        void CreateProduct(ProductItem product);
        void UpdateProduct(ProductItem product);
        void DeleteProduct(string id);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;

        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CategoryItem> GetCategories()
        {
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            var productCounts = _dbContext.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionary(g => g.CategoryId, g => g.Count);

            foreach (var cat in categories)
            {
                cat.ProductCount = productCounts.TryGetValue(cat.Id, out var count) ? count : 0;
            }

            return categories;
        }

        public List<ProductItem> GetProducts(string? categorySlug = null, bool? featuredOnly = null)
        {
            var query = _dbContext.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(categorySlug))
            {
                var category = _dbContext.Categories.AsNoTracking()
                    .FirstOrDefault(c => c.Slug.ToLower() == categorySlug.ToLower());
                
                if (category != null)
                {
                    query = query.Where(p => p.CategoryId == category.Id);
                }
            }

            if (featuredOnly.HasValue && featuredOnly.Value)
            {
                query = query.Where(p => p.IsFeatured);
            }

            return query.ToList();
        }

        public ProductItem? GetProductBySlug(string slug)
        {
            return _dbContext.Products.AsNoTracking()
                .FirstOrDefault(p => p.Slug.ToLower() == slug.ToLower());
        }

        public ProductItem? GetProductById(string id)
        {
            return _dbContext.Products.AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public void CreateProduct(ProductItem product)
        {
            var category = _dbContext.Categories.Find(product.CategoryId);
            if (category != null)
            {
                product.CategoryName = category.Name;
            }

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public void UpdateProduct(ProductItem product)
        {
            var existing = _dbContext.Products.Find(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Slug = product.Slug;
                existing.CategoryId = product.CategoryId;
                var category = _dbContext.Categories.Find(product.CategoryId);
                existing.CategoryName = category?.Name ?? product.CategoryName;
                existing.ShortDescription = product.ShortDescription;
                existing.FullDescription = product.FullDescription;
                existing.ImageUrl = product.ImageUrl;
                existing.Badge = product.Badge;
                existing.IsFeatured = product.IsFeatured;
                existing.Features = product.Features;
                existing.Specifications = product.Specifications;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteProduct(string id)
        {
            var existing = _dbContext.Products.Find(id);
            if (existing != null)
            {
                _dbContext.Products.Remove(existing);
                _dbContext.SaveChanges();
            }
        }
    }
}
