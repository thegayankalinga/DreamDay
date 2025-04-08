using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels.Planner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize(Roles = UserRoles.Planner)]
    public class PlannerController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IVenueRepository _venueRepository;

        public PlannerController(
            IUserProfileRepository userProfileRepository,
            IUserRepository userRepository,
            IChecklistRepository checklistRepository,
            IBudgetRepository budgetRepository,
            IMessageRepository messageRepository,
            IVenueRepository venueRepository)
        {
            _userProfileRepository = userProfileRepository;
            _userRepository = userRepository;
            _checklistRepository = checklistRepository;
            _budgetRepository = budgetRepository;
            _messageRepository = messageRepository;
            _venueRepository = venueRepository;
        }

        // GET: Planner/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            await _userProfileRepository.InitializeAsync();
            var currentUser = _userProfileRepository.CurrentUser;
            var plannerProfile = _userProfileRepository.PlannerProfile;

            if (currentUser == null || plannerProfile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get all assigned couples
            var assignedCouples = await GetAssignedCouples(currentUser.Id);
            
            var viewModel = new PlannerDashboardViewModel
            {
                PlannerName = $"{currentUser.FirstName} {currentUser.LastName}",
                TotalWeddingsCount = assignedCouples.Count,
                UpcomingWeddingsCount = assignedCouples.Count(c => c.WeddingDate > DateOnly.FromDateTime(DateTime.Now)),
                PastWeddingsCount = assignedCouples.Count(c => c.WeddingDate <= DateOnly.FromDateTime(DateTime.Now)),
                AssignedWeddings = await GetWeddingCards(assignedCouples),
                RecentActivities = await GetRecentActivities(assignedCouples)
            };

            return View(viewModel);
        }

        private async Task<List<CoupleProfile>> GetAssignedCouples(string plannerId)
        {
            // This could be optimized with a direct repository method
            var allCouples = await _userRepository.GetAllAsync();
            var coupleProfiles = new List<CoupleProfile>();

            foreach (var user in allCouples.Where(u => u.CoupleProfile != null))
            {
                if (user.CoupleProfile?.AcceptedPlannerId == plannerId)
                {
                    coupleProfiles.Add(user.CoupleProfile);
                }
            }

            return coupleProfiles;
        }

        private async Task<List<PlannerWeddingCardViewModel>> GetWeddingCards(List<CoupleProfile> coupleProfiles)
        {
            var weddingCards = new List<PlannerWeddingCardViewModel>();

            foreach (var profile in coupleProfiles)
            {
                var couple = await _userRepository.GetByIdAsync(profile.AppUserId);
                if (couple == null) continue;

                // Get checklist information
                var checklists = await _checklistRepository.GetAllChecklistsAndItemsByUserAsync(profile.AppUserId);
                int totalItems = 0;
                int completedItems = 0;

                foreach (var checklist in checklists)
                {
                    if (checklist.Items != null)
                    {
                        totalItems += checklist.Items.Count;
                        completedItems += checklist.Items.Count(i => i.IsCompleted);
                    }
                }

                // Get budget information
                var budgetCategories = await _budgetRepository.GetAllCategoriesByUserIdAsync(profile.AppUserId);
                decimal totalBudget = budgetCategories.Sum(c => c.AllocatedAmount);
                decimal budgetUtilized = budgetCategories.Sum(c => c.TotalSpent);

                // Get venue name
                string? venueName = null;
                if (profile.VenueId.HasValue)
                {
                    var venue = await _venueRepository.GetByIdAsync(profile.VenueId.Value);
                    venueName = venue?.Name;
                }

                // Get unread message count
                int unreadMessageCount = await _messageRepository.GetUnreadMessageCountAsync(profile.AppUserId);

                // Determine wedding status
                string status = DetermineWeddingStatus(profile.WeddingDate);

                weddingCards.Add(new PlannerWeddingCardViewModel
                {
                    CoupleProfileId = profile.Id,
                    CoupleName = $"{couple.FirstName} & {profile.PartnerName}",
                    WeddingDate = profile.WeddingDate,
                    VenueName = venueName,
                    CompletedChecklistItems = completedItems,
                    TotalChecklistItems = totalItems,
                    Budget = totalBudget,
                    BudgetUtilized = budgetUtilized,
                    Status = status,
                    UnreadMessageCount = unreadMessageCount
                });
            }

            // Sort by wedding date (upcoming first)
            return weddingCards.OrderBy(w => w.WeddingDate).ToList();
        }

        private string DetermineWeddingStatus(DateOnly weddingDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var oneMonthBefore = today.AddMonths(1);
            
            if (weddingDate < today)
            {
                return "Completed";
            }
            else if (weddingDate <= oneMonthBefore)
            {
                return "InProgress";
            }
            else
            {
                return "Upcoming";
            }
        }

        private async Task<List<RecentActivityViewModel>> GetRecentActivities(List<CoupleProfile> coupleProfiles)
        {
            var activities = new List<RecentActivityViewModel>();
            
            // For initial implementation, we'll just show recent message activities
            // This could be expanded to include checklist, budget, and guest updates
            foreach (var profile in coupleProfiles)
            {
                var couple = await _userRepository.GetByIdAsync(profile.AppUserId);
                if (couple == null) continue;

                string coupleName = $"{couple.FirstName} & {profile.PartnerName}";
                
                // Get recent messages
                var threads = await _messageRepository.GetUserThreadsAsync(profile.AppUserId);
                foreach (var thread in threads.Take(3)) // Limit to 3 most recent threads per couple
                {
                    var messages = await _messageRepository.GetThreadMessagesAsync(thread.Id);
                    var latestMessage = messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
                    
                    if (latestMessage != null && (DateTime.Now - latestMessage.CreatedAt).TotalDays < 7) // Show activities from last 7 days only
                    {
                        activities.Add(new RecentActivityViewModel
                        {
                            Description = $"New message in thread: {thread.Subject}",
                            ActivityTime = latestMessage.CreatedAt,
                            CoupleName = coupleName,
                            CoupleProfileId = profile.Id,
                            ActivityType = "Message"
                        });
                    }
                }
            }
            
            // Sort by most recent activity first
            return activities.OrderByDescending(a => a.ActivityTime).Take(10).ToList();
        }

        // GET: Planner/WeddingDetails/5
        public async Task<IActionResult> WeddingDetails(int id)
        {
            // This would be implemented in the next phase
            return View();
        }

        // GET: Planner/ManageChecklists/5
        public async Task<IActionResult> ManageChecklists(int id)
        {
            // This would be implemented in the next phase
            return View();
        }
    }
}