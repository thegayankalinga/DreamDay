using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.ViewModels;

public class BudgetViewModel
{
    //Summary
    public decimal? TotalAllocated { get; set; }
    public decimal? TotalUtilized { get; set; }

    //All Categories
    public List<BudgetCategorySummaryViewModel> Categories { get; set; } = new();

    //New Category Form
    public AddBudgetCategoryViewModel NewCategory { get; set; } = new();

    //New Expense Form
    public AddExpenseVeiwModel? NewExpense { get; set; }

    //Category Dropdown List for Expense Form
    public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
}