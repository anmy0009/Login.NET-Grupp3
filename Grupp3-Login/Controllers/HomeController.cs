using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Grupp3_Login.Models; // Se till att `LoginRequest`-modellen finns här

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 🏠 Visa loginformuläret
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Admin()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index");
        }

        return View();
    }

    // ✅ Hantera inloggning via API:et
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model); // Om formuläret är felaktigt, visa det igen
        }

        // 🔹 Skicka loginförfrågan till API:et
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7200/api/Authentication/login", model);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Felaktigt användarnamn eller lösenord.";
            return View("Index");
        }

        // 🔹 Läs svaret och spara JWT-token & roll i sessionen
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        HttpContext.Session.SetString("JWTToken", result.Token);
        HttpContext.Session.SetString("UserRole", result.Role);

        if (result.Role == "Admin")
        {
            return RedirectToAction("Admin");
        }

        return RedirectToAction("Dashboard"); // Skicka användaren till en skyddad sida
    }

    // 🔒 Skyddad vy (Dashboard)
    [Authorize] // Kräver autentisering
    public IActionResult Dashboard()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index"); // Skicka tillbaka till login om ej inloggad
        }

        return View();
    }

    // 🚪 Logga ut
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Rensa sessionen
        return RedirectToAction("Index");
    }
}
