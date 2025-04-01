using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers.Dashboard
{
    public class CoupleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
