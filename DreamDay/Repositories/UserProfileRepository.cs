using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class UserProfileRepository: IUserProfileRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
        
     public string? UserType { get; private set; }
     public CoupleProfile? CoupleProfile { get; private set; }
     public PlannerProfile? PlannerProfile { get; private set; }
     public AppUser? CurrentUser { get; private set; }
        
        public UserProfileRepository(
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        
        public async Task InitializeAsync()
        {
            if (_httpContextAccessor.HttpContext == null || 
                _httpContextAccessor.HttpContext.User.Identity is not { IsAuthenticated: true })
                return;
        
            CurrentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (CurrentUser == null) return;
            
            // Determine user type from roles
            if (_httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Couple))
            {
                UserType = UserRoles.Couple;
                CoupleProfile = await _context.CoupleProfiles
                    .FirstOrDefaultAsync(c => c.AppUserId == CurrentUser.Id);
            }
            else if (_httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Planner))
            {
                UserType = UserRoles.Planner;
                PlannerProfile = await _context.PlannerProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == CurrentUser.Id);
            }
            else if (_httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin))
            {
                UserType = UserRoles.Admin;
                
            }
        }
    
}