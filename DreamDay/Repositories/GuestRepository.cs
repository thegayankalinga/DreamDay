using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class GuestRepository: IGuestRepository
{
    private readonly ApplicationDbContext _context;

    public GuestRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<List<Guest>> GetAllGuestUsingAppUserIdAsync(string userId)
    {
        return await _context.Guests
            .Where(g => g.AppUserId == userId)
            .ToListAsync();
    }

    public async Task<List<Guest>> GetAllGuestUsingCoupleId(int coupleId)
    {
        return await _context.Guests
            .Where(couple => couple.CoupleProfileId == coupleId)
            .ToListAsync();
    }
    
    public async Task<Guest?> GetByGuestIdAsync(int id)
    {
        return await _context.Guests.FindAsync(id);
    }
    
    public async Task<bool> AddGuestAsync(Guest guest)
    {
        _context.Guests.Add(guest);
        var result = await _context.SaveChangesAsync();

        if (result != 1)
        {
            return false;
        }
        return true;
    }
    
    public async Task<bool> UpdateGuestAsync(Guest guest, int guestId)
    {
        var existingGuest = await _context.Guests.FindAsync(guestId);
        if (existingGuest == null)
        {
            return false;
        }
        
        existingGuest.GuestName = guest.GuestName;
        existingGuest.Email = guest.Email;
        existingGuest.Phone = guest.Phone;
        existingGuest.MealPreference = guest.MealPreference;
        existingGuest.RsvpStatus = guest.RsvpStatus;
      
        var result = await _context.SaveChangesAsync();
        if (result != 1)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteGuestAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest != null)
        {
            _context.Guests.Remove(guest);
            var result = await _context.SaveChangesAsync();
            if (result != 1)
            {
                return false;
            }
        }
        return true;
    }
}