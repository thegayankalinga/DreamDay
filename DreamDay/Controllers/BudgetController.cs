using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.Controllers;

public class BudgetController : Controller
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public BudgetController
    (
        IBudgetRepository budgetRepository,
        IUserProfileRepository userProfileRepository
    )
    {
        _budgetRepository = budgetRepository;
        _userProfileRepository = userProfileRepository;
    }
    // GET
    public async  Task<IActionResult> Index()
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if(currentUser == null) return RedirectToAction("Login", "Account");

        List<BudgetCategory> categories =
            await _budgetRepository.GetAllCategoriesByUserIdAsync(currentUser.Id);

        List<BudgetCategorySummaryViewModel> summaryViewModels = categories
            .Select(c => new BudgetCategorySummaryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                AllocatedAmount = c.AllocatedAmount,
                TotalSpent = c.Expenses?.Sum(e => e.Amount) ?? 0
            })
            .ToList();



        var viewModel = new BudgetViewModel
        {
            TotalAllocated = currentUser.TotalAllocated,
            TotalUtilized = currentUser.TotalUtilized,

            Categories = summaryViewModels,
            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()

        };
        return View(viewModel);
    }


    //Cateogories
    #region Category

    [HttpPost]
    public async Task<IActionResult> AddCategory(BudgetViewModel model)
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        var category = new BudgetCategory
        {
            Name = model.NewCategory.Name,
            AllocatedAmount = model.NewCategory.AllocatedAmount,
            UserId = currentUser.Id,
            
        };

        await _budgetRepository.AddCategoryAsync(category, currentUser.Id);
        return RedirectToAction("Index");
    }


    #endregion
    
    #region Expense

    [HttpPost]
    public async Task<IActionResult> AddExpense(BudgetViewModel model)
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            // You might want to reload model data before returning the view
            return RedirectToAction("Index");
        }

        if (model.NewExpense?.BudgetCategoryId == null)
        {
            Console.WriteLine("Budget Category Id is empty");
            return RedirectToAction("Index");
        }

        var expense = new Expense
        {
            BudgetCategoryId = model.NewExpense.BudgetCategoryId,
            Description = model.NewExpense.Description,
            Amount = model.NewExpense.Amount,
            ExpenseDate = DateTime.Now,
        };

        var result = await _budgetRepository.AddExpenseAsync(expense, model.NewExpense.BudgetCategoryId);
        if (!result)
        {
            Console.WriteLine("Error in expense creation");
        }
        return RedirectToAction("Index");
    }
    
    #endregion
}