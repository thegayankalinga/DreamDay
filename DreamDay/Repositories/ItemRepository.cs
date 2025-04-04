using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class ItemRepository(ApplicationDbContext context): IItemRepository
{
    public async Task<List<ChecklistItem>> GetAllChecklistItemsByChecklistIdAsync(int checklistId)
    {
        var items = await context.ChecklistItems
            .Where(list => list.WeddingChecklistId == checklistId)
            .ToListAsync();

        return items;
    }

    public async Task<ChecklistItem?> GetChecklistItemByIdAsync(int checklistItemId)
    {
        var item = await context.ChecklistItems
            .Where(list => list.Id == checklistItemId)
            .FirstOrDefaultAsync();

        return item;
    }

    public bool AddChecklistItem(ChecklistItem item)
    {
        try
        {
            context.ChecklistItems.Add(item);
            return context.SaveChanges() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> UpdateChecklistItem(ChecklistItem item, int checklistItemId)
    {
        try
        {
            var existingItem = await context.ChecklistItems.FirstOrDefaultAsync(i => i.Id == checklistItemId);
            if (existingItem == null)
            {
                return false;
            }

            context.Entry(existingItem).CurrentValues.SetValues(item);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> UpdateDueDate(DateTime dueDate, int checklistItemId)
    {
        try
        {
            var existingItem = await context.ChecklistItems.FirstOrDefaultAsync(i => i.Id == checklistItemId);
            if (existingItem == null)
            {
                return false;
            }

            context.Entry(existingItem).CurrentValues.SetValues(dueDate);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public async Task<bool> DeleteChecklistItem(int checklistItemId)
    {
        var existingItem = await context.ChecklistItems.FirstOrDefaultAsync(i => i.Id == checklistItemId);
        if (existingItem == null)
        {
            return false;
        }
        
        context.ChecklistItems.Remove(existingItem);
        await context.SaveChangesAsync();
        return true;
    }

    
}