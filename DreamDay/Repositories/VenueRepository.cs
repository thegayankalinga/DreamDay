using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class VenueRepository: IVenueRepository
{
    private readonly ApplicationDbContext _context;

    public VenueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venue>> GetAllVenuesAsync()
    {
        return await _context.Venues.ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(int id)
    {
        return await _context.Venues.FindAsync(id);
    }

    public async Task<bool> AddAsync(Venue venue)
    {
        _context.Venues.Add(venue);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> UpdateAsync(Venue venue, int venueId)
    {
        var existingVenue = await _context.Venues.FindAsync(venueId);
        if (existingVenue == null)
            return false;

        existingVenue.Name = venue.Name;
        existingVenue.Address = venue.Address;

        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteAsync(int venueId)
    {
        var venue = await _context.Venues.FindAsync(venueId);
        if (venue == null)
            return false;

        _context.Venues.Remove(venue);
        return await _context.SaveChangesAsync() == 1;
    }
}