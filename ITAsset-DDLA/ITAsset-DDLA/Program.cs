using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Services.Abstract;
using ddla.ITApplication.Services.Concrete;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<ddlaAppDBContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ddlaUser, IdentityRole>()
            .AddEntityFrameworkStores<ddlaAppDBContext>();

        builder.Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation();
        builder.Services.AddRazorPages();

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IStockService, StockProductService>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters = null;
            options.User.RequireUniqueEmail = false;
        });

        var app = builder.Build();

        app.UseStaticFiles();

        app.MapControllerRoute(
            name: "Areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
