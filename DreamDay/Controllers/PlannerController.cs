using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers.Dashboard
{
    public class PlannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
