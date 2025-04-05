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

    public async Task<bool> AddChecklistItemAsync(ChecklistItem item)
    {
        try
        {
            await context.ChecklistItems.AddAsync(item);
            return await context.SaveChangesAsync() > 0;
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
            
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            existingItem.DueDate = item.DueDate;
            existingItem.IsCompleted = item.IsCompleted;
            existingItem.CheckInDate = item.CheckInDate;
            existingItem.WeddingChecklistId = item.WeddingChecklistId;
            
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> UpdateConfirmStatus(ChecklistItem item, int checklistItemId)
    {
        try
        {
            var existingItem = await context.ChecklistItems.FirstOrDefaultAsync(i => i.Id == checklistItemId);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.IsCompleted = item.IsCompleted;
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