using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class BudgetController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    // public Task<IActionResult> NewExpense()
    // {
    //     return View();
    // }
        
}