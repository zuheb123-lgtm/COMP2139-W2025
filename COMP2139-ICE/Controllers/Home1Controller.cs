using Microsoft.AspNetCore.Mvc;

namespace COMP2139_ICE.Controllers;

public class Home1Controller : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}