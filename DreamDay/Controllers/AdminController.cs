using DreamDay.Data;
using DreamDay.Data.Enums;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWeddingEventRepository _weddingEventRepository;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(
            IUserRepository userRepository,
            IVendorRepository vendorRepository,
            IVenueRepository venueRepository,
            IBudgetRepository budgetRepository,
            IWeddingEventRepository weddingEventRepository, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _vendorRepository = vendorRepository;
            _venueRepository = venueRepository;
            _budgetRepository = budgetRepository;
            _weddingEventRepository = weddingEventRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get dashboard statistics
          
            var vendors = await _vendorRepository.GetAllVendorsAsync();
            var venues = await _venueRepository.GetAllVenuesAsync();
            
            
            var users = await _userManager.Users.Include(u => u.CoupleProfile).ToListAsync();

            var dashboardUsers = new List<DashboardUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                dashboardUsers.Add(new DashboardUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    CoupleProfile = user.CoupleProfile,
                    Roles = roles.ToList()
                });
            }

            var dashboardViewModel = new AdminDashboardViewModel
            {
                TotalUsers = dashboardUsers.Count,
                TotalCouples = dashboardUsers.Count(u => u.Roles.Contains("Couple")),
                TotalPlanners = dashboardUsers.Count(u => u.Roles.Contains("Planner")),
                TotalVendors = vendors.Count,
                TotalVenues = venues.Count,
                RecentUsers = dashboardUsers
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToList(),
                UpcomingWeddings = dashboardUsers
                    .Where(u => u.CoupleProfile?.WeddingDate >= DateOnly.FromDateTime(DateTime.Today))
                    .OrderBy(u => u.CoupleProfile!.WeddingDate)
                    .Take(5)
                    .ToList()
            };

            return View(dashboardViewModel);
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .Include(u => u.PlannerProfile)
                .ToListAsync();

            var viewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                viewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles,
                    IsApproved = user.PlannerProfile?.IsApproved,
                    CreatedAt = user.CreatedAt
                });
            }

            return View(viewModels);
        }

        // Vendor Management
        public async Task<IActionResult> Vendors()
        {
            var vendors = await _vendorRepository.GetAllVendorsAsync();
            ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
            return View(vendors);
        }

        // Venue Management
        public async Task<IActionResult> Venues()
        {
            var venues = await _venueRepository.GetAllVenuesAsync();
            return View(venues);
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            var reportViewModel = new AdminReportViewModel();

            // --- Popular venues ---
            var venues = await _venueRepository.GetAllVenuesAsync();
            var events = await _weddingEventRepository.GetAllAsync();

            reportViewModel.PopularVenues = venues
                .Select(v => new VenuePopularityViewModel
                {
                    VenueName = v.Name,
                    BookingCount = events.Count(e => e.VenueId == v.Id)
                })
                .OrderByDescending(v => v.BookingCount)
                .Take(10)
                .ToList();

            // --- Average Budget Report ---
            var users = await _userManager.Users
                .Include(u => u.CoupleProfile) // optional if you need CoupleProfile info
                .ToListAsync();

            var couples = new List<AppUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Couple"))
                    couples.Add(user);
            }

            decimal totalBudget = 0;
            int budgetCount = 0;

            foreach (var couple in couples)
            {
                var categories = await _budgetRepository.GetAllCategoriesByUserIdAsync(couple.Id);
                if (categories.Any())
                {
                    totalBudget += categories.Sum(c => c.AllocatedAmount);
                    budgetCount++;
                }
            }

            reportViewModel.AverageBudget = budgetCount > 0 ? totalBudget / budgetCount : 0;

            // --- Monthly Registration Report ---
            reportViewModel.MonthlyRegistrations = users
                .Where(u => u.CreatedAt.Month == DateTime.Now.Month)
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month })

                .Select(g => new MonthlyRegistrationViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .Take(12)
                .ToList();

            return View(reportViewModel);
        }

    }
}