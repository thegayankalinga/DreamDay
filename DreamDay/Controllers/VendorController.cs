using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class VendorController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}