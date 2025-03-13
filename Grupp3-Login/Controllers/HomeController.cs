using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grupp3_Login.Models;

namespace Grupp3_Login.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}