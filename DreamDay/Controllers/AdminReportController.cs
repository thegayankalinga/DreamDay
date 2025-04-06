using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class AdminReportController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}