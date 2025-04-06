using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class BudgetRepository: IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<List<BudgetCategory>> GetAllCategoriesByUserIdAsync(string userId)
    {
        return await _context.BudgetCategories
            .Where(b => b.UserId == userId)
            .Include(b => b.Expenses)
            .ToListAsync();
    }

    public async Task<BudgetCategory?> GetCategoryByIdAsync(int id)
    {
        return await _context.BudgetCategories
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> AddCategoryAsync(BudgetCategory category, string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        
        await _context.BudgetCategories.AddAsync(category);
        var result = await _context.SaveChangesAsync();
        return result == 1;
    }

    public async Task<bool> AddExpenseAsync(Expense expense, int categoryId)
    {
        var category = await _context.BudgetCategories.FindAsync(categoryId);
        if (category == null) return false;
        
        await _context.Expenses.AddAsync(expense);
        var result = await _context.SaveChangesAsync();
        return result == 1;
    }

    public async Task<bool> UpdateExpenseAsync(Expense expense, int id)
    {
        var existingExpense = await _context.Expenses.FindAsync(id);
        if (existingExpense == null)
        {
            return false;
        }

        existingExpense.Description = expense.Description;
        existingExpense.Amount = expense.Amount;
        existingExpense.ExpenseDate = expense.ExpenseDate;
        existingExpense.BudgetCategoryId = expense.BudgetCategoryId;

        var result = await _context.SaveChangesAsync();
        return result == 1;
    }

    public async Task<bool> UpdateBudgetCategoryAsync(BudgetCategory budgetCategory, int id)
    {
        var existingCategory = await _context.BudgetCategories.FindAsync(id);
        if (existingCategory == null)
        {
            return false;
        }

        existingCategory.Name = budgetCategory.Name;
        existingCategory.AllocatedAmount = budgetCategory.AllocatedAmount;

        var result = await _context.SaveChangesAsync();
        return result == 1;
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return false;
        
        
        _context.Expenses.Remove(expense);
        var result = await _context.SaveChangesAsync();
        return result == 1;

    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.BudgetCategories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category != null)
        {
            if (category.Expenses != null) _context.Expenses.RemoveRange(category.Expenses); // remove child items first
            _context.BudgetCategories.Remove(category);

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        return true;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> IsBudgetExceeded(int categoryId)
    {
        var category = await GetCategoryByIdAsync(categoryId);

        if (category == null || category.Expenses == null)
            return false;

        var totalExpenses = category.Expenses.Sum(e => e.Amount);
        return totalExpenses > category.AllocatedAmount;
    }

}