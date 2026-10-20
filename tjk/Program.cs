using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using tjk.Data;

namespace tjk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<tjk.Services.IProductService, tjk.Services.ProductService>();
            builder.Services.Configure<tjk.Models.CompanySettings>(builder.Configuration.GetSection("CompanySettings"));

            // Health Checks for Cloud / Container Deployment
            builder.Services.AddHealthChecks();

            // Authentication & Authorization for Admin Area
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/admin/login";
                    options.LogoutPath = "/admin/logout";
                    options.AccessDeniedPath = "/admin/login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                });
            builder.Services.AddAuthorization();

            // Database Provider Setup (MySQL / SQLite)
            var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
            if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
            {
                var mySqlConn = builder.Configuration.GetConnectionString("MySqlConnection") 
                    ?? "Server=localhost;Port=3306;Database=tjk_db;User=root;Password=;CharSet=utf8mb4;";
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(mySqlConn, serverVersion, mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                    }));
            }
            else
            {
                var sqliteConn = builder.Configuration.GetConnectionString("SqliteConnection") 
                    ?? "Data Source=tjk_inquiries.db";
                if (sqliteConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    var dataSource = sqliteConn.Substring("Data Source=".Length).Trim();
                    if (!Path.IsPathRooted(dataSource))
                    {
                        var fullPath = Path.Combine(builder.Environment.ContentRootPath, dataSource);
                        var dir = Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        sqliteConn = $"Data Source={fullPath}";
                    }
                }
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(sqliteConn));
            }

            var app = builder.Build();

            // Auto-create database & tables if not existing, seed catalog data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                    tjk.Data.DbInitializer.Seed(db);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            // Serve uploaded images stored in persistent /data/uploads directory
            var dataUploadsDir = Path.Combine(app.Environment.ContentRootPath, "data", "uploads");
            if (!Directory.Exists(dataUploadsDir))
            {
                Directory.CreateDirectory(dataUploadsDir);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(dataUploadsDir),
                RequestPath = "/data/uploads"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
