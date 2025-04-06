using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IBudgetRepository
{
    Task<List<BudgetCategory>> GetAllCategoriesByUserIdAsync(string userId);
    Task<BudgetCategory?> GetCategoryByIdAsync(int id);
    Task<bool> AddCategoryAsync(BudgetCategory category, string userId);
    Task<bool> AddExpenseAsync(Expense expense, int categoryId);
    Task<bool> UpdateExpenseAsync(Expense expense, int id);
    Task<bool> UpdateBudgetCategoryAsync(BudgetCategory budgetCategory, int id);
    Task<bool> DeleteExpenseAsync(int id);
    Task<bool> DeleteCategoryAsync(int id);
    Task SaveAsync();
    Task<bool> IsBudgetExceeded(int categoryId);
}