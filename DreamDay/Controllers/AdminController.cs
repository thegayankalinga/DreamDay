using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers.Dashboard
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
