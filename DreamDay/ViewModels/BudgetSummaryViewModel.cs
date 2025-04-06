using DreamDay.Models;

namespace DreamDay.ViewModels;

public class BudgetSummaryViewModel
{
    public decimal TotalAllocated { get; set; }
    public decimal TotalSpent { get; set; }
    public bool IsOverBudget { get; set; }
    
    public List<BudgetCategory>? Top3Categories { get; set; }
}