using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class PlannerReportController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}