using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppUser>> GetAllAsync()
    {
        return await _context.Users.OrderByDescending(u => u.Id).ToListAsync();
    }

    public async Task<AppUser?> GetByIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UpdateAsync(AppUser user, string userId)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        

        if (existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            existingUser.PhoneNumber = user.PhoneNumber;
            
            await _context.SaveChangesAsync();
            return true;
        }


        
        return false;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task TogglePlannerActivationAsync(string userId)
    {
        var planner = await _context.PlannerProfiles
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (planner != null)
        {
            planner.IsApproved = !planner.IsApproved;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool?> GetPlannerActivationStatusAsync(string userId)
    {
        var planner = await _context.PlannerProfiles
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        return planner?.IsApproved;
    } 
}