using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult CoupleDashboard()
    {
        
        return View("CoupleDashboard");
    }
    
    public IActionResult PlannerDashboard()
    {
        
        return View("PlannerDashboard");
    }

    public IActionResult AdminDashboard()
    {
        return View("AdminDashboard");
    }
}