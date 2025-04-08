using DreamDay.Data;
using DreamDay.Data.Enums;
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
    
    public async Task<List<AppUser>> GetAllAvailablePlannersAsync()
    {
        return await _context.Users
            .Include(u => u.PlannerProfile)
            .Where(u => u.PlannerProfile != null && u.PlannerProfile.IsApproved)
            .ToListAsync();
    }

    public async Task<bool> RequestPlannerAsync(string coupleId, string plannerId, string message)
    {
        var couple = await _context.Users
            .Include(u => u.CoupleProfile)
            .FirstOrDefaultAsync(u => u.Id == coupleId);
        
        if (couple?.CoupleProfile == null)
            return false;
    
        // Check if a request already exists for this planner
        var existingRequest = await _context.Set<PlannerRequest>()
            .FirstOrDefaultAsync(pr => pr.CoupleProfileId == couple.CoupleProfile.Id && 
                                       pr.PlannerId == plannerId);
    
        if (existingRequest != null)
        {
            // If the request was declined, we can reactivate it
            if (existingRequest.Status == PlannerRequestStatus.Declined)
            {
                existingRequest.Status = PlannerRequestStatus.Requested;
                existingRequest.RequestDate = DateTime.Now;
                existingRequest.Message = message;
            }
            // Otherwise, don't create a duplicate request
            else
            {
                return true;
            }
        }
        else
        {
            // Create a new request
            var request = new PlannerRequest
            {
                CoupleProfileId = couple.CoupleProfile.Id,
                PlannerId = plannerId,
                Message = message,
                RequestDate = DateTime.Now,
                Status = PlannerRequestStatus.Requested
            };
        
            _context.Add(request);
        }
    
        await _context.SaveChangesAsync();
        return true;
    }
    
    
    public async Task<List<PlannerRequest>> GetCoupleRequestsAsync(int coupleProfileId)
    {
        return await _context.Set<PlannerRequest>()
            .Include(pr => pr.Planner)
            .Where(pr => pr.CoupleProfileId == coupleProfileId)
            .ToListAsync();
        
    }
    
    public async Task<bool> CancelPlannerRequestAsync(int requestId)
    {
        var request = await _context.Set<PlannerRequest>().FindAsync(requestId);
        if (request == null)
            return false;
        
        _context.Remove(request);
        await _context.SaveChangesAsync();
        return true;
    }
    
   
}