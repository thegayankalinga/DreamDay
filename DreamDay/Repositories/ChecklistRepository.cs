using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class ChecklistRepository(ApplicationDbContext context) : IChecklistRepository
{
    
    public async Task<List<Checklist>> GetAllChecklistsByUserAsync(string userId)
    {
        return await context.Checklists
            .Where(user => user.AppUserId == userId)
            .ToListAsync();
    }
    
    public async Task<List<Checklist>> GetAllChecklistsAndItemsByUserAsync(string userId)
    {
        return await context.Checklists
            .Where(user => user.AppUserId == userId)
            .Include(checklist => checklist.Items)
            .ToListAsync();

    }

    public async Task<Checklist?> GetChecklistByIdAsync(int id)
    {
        var checklist = await context.Checklists
            .Include(check => check.Items)
            .FirstOrDefaultAsync(check => check.Id == id);

        return checklist;
    }

    public async Task CreateChecklistAsync(Checklist checklist)
    {
        context.Checklists.Add(checklist);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateChecklistAsync(Checklist checklist)
    {
        try
        {
            var existingChecklist = await context.Checklists.FindAsync(checklist.Id);
            if (existingChecklist == null)
            {
                return false;
            }
        
            context.Entry(existingChecklist).CurrentValues.SetValues(checklist);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteChecklistAsync(int id)
    {
        var checklist = await context.Checklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    
        if (checklist == null)
        {
            return false;
        }
    
        context.Checklists.Remove(checklist);
        await context.SaveChangesAsync();
        return true;
    }

    
}