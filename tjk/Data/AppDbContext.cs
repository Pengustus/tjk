using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using tjk.Models;

namespace tjk.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<CategoryItem> Categories => Set<CategoryItem>();
        public DbSet<ProductItem> Products => Set<ProductItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inquiry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Company).HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SelectedProduct).HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Ново");
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<CategoryItem>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Slug).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Description).HasMaxLength(500);
            });

            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            var stringDictComparer = new ValueComparer<Dictionary<string, string>>(
                (d1, d2) => d1 != null && d2 != null ? d1.Count == d2.Count && !d1.Except(d2).Any() : d1 == d2,
                d => d.Aggregate(0, (a, p) => HashCode.Combine(a, p.Key.GetHashCode(), p.Value.GetHashCode())),
                d => new Dictionary<string, string>(d));

            modelBuilder.Entity<ProductItem>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Slug).IsRequired().HasMaxLength(200);
                entity.Property(p => p.CategoryId).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ShortDescription).HasMaxLength(500);
                entity.Property(p => p.Badge).HasMaxLength(100);

                entity.Property(p => p.Features)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(stringListComparer);

                entity.Property(p => p.Specifications)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => string.IsNullOrEmpty(v) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                    )
                    .Metadata.SetValueComparer(stringDictComparer);
            });
        }
    }
}
