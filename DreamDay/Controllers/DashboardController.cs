using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}