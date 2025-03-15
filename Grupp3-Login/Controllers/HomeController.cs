using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grupp3_Login.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Grupp3_Login.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() { 
            return View();
        }

        // Login GET-action (visar inloggningssidan)
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; // Sätt returnUrl i ViewBag för att kunna använda den i vyn
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Account account, string returnUrl)
        {
            // Kolla användarnamn och lösenord
            bool accountValid = account.userName == "superadmin" && account.password == "Robert54321";

            // Fel användarnamn eller lösenord
            if (accountValid == false)
            {
                ViewBag.ErrorMessage = "Login failed: Wrong username or password";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            // Korrekt användarnamn och lösenord, logga in användaren
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, account.userName));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            // Ifall vi inte har en returnUrl, gå till Home
            if (String.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Admin", "Home");
            }

            // Gå tillbaka via returnUrl
            return Redirect(returnUrl);
        }

        [Authorize]
        public IActionResult Admin()
        {
            return View();
        }

    }
}
