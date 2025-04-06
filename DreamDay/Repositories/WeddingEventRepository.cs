using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class WeddingEventRepository : IWeddingEventRepository
{
    private readonly ApplicationDbContext _context;

    public WeddingEventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WeddingEvent>> GetAllByUserIdAsync(string userId)
    {
        return await _context.WeddingEvents
            .Where(e => e.UserId == userId)
            .Include(e => e.Venue)
            .Include(e => e.Checklist)
            .Include(e => e.WeddingEventVendors)
            .ThenInclude(wev => wev.Vendor)
            .ToListAsync();
    }


    public async Task<WeddingEvent?> GetByIdAsync(int id)
    {
        return await _context.WeddingEvents
            .Include(e => e.Venue)
            .Include(e => e.Checklist)
            .Include(e => e.WeddingEventVendors)
                .ThenInclude(wev => wev.Vendor)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> AddAsync(WeddingEvent weddingEvent, string userId)
    {
        _context.WeddingEvents.Add(weddingEvent);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(WeddingEvent weddingEvent, int eventId)
    {
        var existing = await _context.WeddingEvents.FindAsync(eventId);
        if (existing == null) return false;

        existing.Title = weddingEvent.Title;
        existing.Description = weddingEvent.Description;
        existing.StartTime = weddingEvent.StartTime;
        existing.EndTime = weddingEvent.EndTime;
        existing.Location = weddingEvent.Location;
        existing.ChecklistId = weddingEvent.ChecklistId;
        existing.VenueId = weddingEvent.VenueId;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.WeddingEvents.FindAsync(id);
        if (existing != null)
        {
            _context.WeddingEvents.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
    
    public async Task<List<Vendor>> GetVendorsByEventIdAsync(int eventId)
    {
        return await _context.WeddingEventVendors
            .Where(wev => wev.TimelineEventId == eventId)
            .Include(wev => wev.Vendor)
            .Select(wev => wev.Vendor!)
            .ToListAsync();
    }

    public async Task<bool> AssignVendorsToEventAsync(int eventId, List<int> vendorIds)
    {
        // Remove existing vendor mappings
        var existingMappings = await _context.WeddingEventVendors
            .Where(wev => wev.TimelineEventId == eventId)
            .ToListAsync();

        _context.WeddingEventVendors.RemoveRange(existingMappings);

        // Add new mappings
        foreach (var vendorId in vendorIds)
        {
            _context.WeddingEventVendors.Add(new WeddingEventVendor
            {
                TimelineEventId = eventId,
                VendorId = vendorId
            });
        }

        return await _context.SaveChangesAsync() > 0;
    }
}
