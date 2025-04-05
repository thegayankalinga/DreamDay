using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
