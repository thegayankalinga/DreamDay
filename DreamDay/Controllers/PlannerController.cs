using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers
{
    public class PlannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
