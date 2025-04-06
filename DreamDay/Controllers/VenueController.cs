using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class VenueController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}