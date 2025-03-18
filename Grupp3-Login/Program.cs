using Grupp3_Login.Models;
using Grupp3_Login.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Grupp3_Login
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient<AccountService>();

            // 🔹 Lägg till autentisering och auktorisering
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = "/Home/Login");

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("requireAdmin", policy =>
                    policy.RequireClaim("RoleId", "1"));
            });

            // 🔹 Lägg till sessionshantering
            builder.Services.AddSession();

            // 🔹 Lägg till controllers & views
            builder.Services.AddControllersWithViews();

            // 🔹 Lägg till SQLite-databasen
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // 🔹 Konfigurera pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // 🔹 Lägg till sessionshantering i pipelinen
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
