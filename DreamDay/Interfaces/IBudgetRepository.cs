using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IBudgetRepository
{
    Task<List<BudgetCategory>> GetAllCategoriesByUserIdAsync();
    Task<BudgetCategory> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(BudgetCategory category);
    Task AddExpenseAsync(Expense expense);
    Task SaveAsync();
}