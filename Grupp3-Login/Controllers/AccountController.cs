using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Grupp3_Login.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

[Authorize(Policy = "requireAdmin")] // 🔐 Endast Admin kan komma åt denna controller
public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // 🔹 Visa lista över alla konton (Hantera konton-sidan)
    public async Task<IActionResult> Index()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Role) // Hämtar även roll-information
            .ToListAsync();

        return View(accounts); // Länkar till rätt vy
    }

    // 🔹 Skapa nytt konto (GET)
    public IActionResult Create()
    {
        return View();
    }

    // 🔹 Skapa nytt konto (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Account account)
    {
        if (ModelState.IsValid)
        {
            _context.Add(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    // 🔹 Redigera konto (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        return View(account);
    }

    // 🔹 Redigera konto (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Account account)
    {
        if (id != account.id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            _context.Update(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    // 🔹 Radera konto (GET: Bekräftelsesida)
    public async Task<IActionResult> Delete(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        return View(account);
    }

    // 🔹 Radera konto (POST)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult RegisterCustomer()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCustomer(Account account)
    {
        if (ModelState.IsValid)
        {
            // Assigna automatiskt role id 3.
            account.roleId = 3;


            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();


            // Omdirigera till home
            return RedirectToAction("Index", "Home");
        }

        return View(account);
    }
}
