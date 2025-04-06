namespace DreamDay.ViewModels;

public class AddExpenseVeiwModel
{
    public int BudgetCategoryId { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}