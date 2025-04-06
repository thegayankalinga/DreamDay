namespace DreamDay.ViewModels;

public class BudgetCategorySummaryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal AllocatedAmount { get; set; }
    public decimal TotalSpent { get; set; }
}