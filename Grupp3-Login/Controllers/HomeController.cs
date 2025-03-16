using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Grupp3_Login.Models;
using Microsoft.AspNetCore.Authorization;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Account account, string returnUrl)
    {
        var user = _context.Accounts.FirstOrDefault(a => a.userName == account.userName && a.password == account.password);

        if (user == null)
        {
            ViewBag.ErrorMessage = "Fel användarnamn eller lösenord";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.userName),
            new Claim("RoleId", user.roleId.ToString()) // 👈 Lagrar roleId som claim
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true, // Håller sessionen vid liv om webbläsaren stängs
                ExpiresUtc = DateTime.UtcNow.AddMinutes(10) // Automatisk utloggning efter 10 min
            });


        if (user.roleId == 1)
        {
            return RedirectToAction("Admin", "Home");
        }

        return RedirectToAction("Admin", "Home");
    }

    [Authorize(Policy = "requireAdmin")] // Endast admin kan komma åt Admin-sidan
    public IActionResult Admin()
    {
        return View();
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home"); // Skickar användaren till startsidan efter utloggning
    }
}
