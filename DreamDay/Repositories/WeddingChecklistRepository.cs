using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class WeddingChecklistRepository : IWeddingChecklistRepository
{
    private readonly ApplicationDbContext _context;

    public WeddingChecklistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeddingChecklist>> GetChecklistsByUserIdAsync(string userId)
    {
        return await _context.WeddingChecklists
            .Include(c => c.Items)
            .Where(c => c.AppUserId == userId)
            .ToListAsync();
    }

    public async Task<WeddingChecklist?> GetChecklistByIdAsync(int id)
    {
        return await _context.WeddingChecklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateChecklistAsync(WeddingChecklist checklist)
    {
        _context.WeddingChecklists.Add(checklist);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateChecklistAsync(WeddingChecklist checklist)
    {
        _context.WeddingChecklists.Update(checklist);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChecklistAsync(int id)
    {
        var checklist = await _context.WeddingChecklists.FindAsync(id);
        if (checklist != null)
        {
            _context.WeddingChecklists.Remove(checklist);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<WeddingChecklistItem?> GetItemByIdAsync(int itemId)
    {
        return await _context.WeddingChecklistItems.FindAsync(itemId);
    }

    public async Task AddItemAsync(WeddingChecklistItem item)
    {
        _context.WeddingChecklistItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(WeddingChecklistItem item)
    {
        _context.WeddingChecklistItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int itemId)
    {
        var item = await _context.WeddingChecklistItems.FindAsync(itemId);
        if (item != null)
        {
            _context.WeddingChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}