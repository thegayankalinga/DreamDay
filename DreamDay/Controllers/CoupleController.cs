using DreamDay.Data.Enums;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using DreamDay.ViewModels.Couple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.Controllers
{
    [Authorize(Policy = "RequireCoupleRole")]
    public class CoupleController : Controller
    {
        
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWeddingEventRepository _weddingRepository;
        private readonly IUserRepository _userRepository;

    public CoupleController(
        IUserProfileRepository userProfileRepository, 
        IChecklistRepository checklistRepository, 
        IItemRepository itemRepository,
        IGuestRepository guestRepository, 
        IBudgetRepository budgetRepository, IWeddingEventRepository weddingRepository,
        IUserRepository userRepository)
    {
        _userProfileRepository = userProfileRepository;
        _checklistRepository = checklistRepository;
        _itemRepository = itemRepository;
        _guestRepository = guestRepository;
        _budgetRepository = budgetRepository;
        _weddingRepository = weddingRepository;
        _userRepository = userRepository;
    }
    
    [Authorize(Policy = "RequireCoupleRole")]
    public async Task<IActionResult> Index()
    {
        var coupleProfile = _userProfileRepository.CoupleProfile;
        // var currentUserRole = _userProfileRepository.UserType;
        var currentUser = _userProfileRepository.CurrentUser;
        if (coupleProfile == null)
        {
            //Console.WriteLine("Couple profile not found", coupleProfile);
            return RedirectToAction("Index", "Home");
        }

        if (currentUser is null)
        {
            Console.WriteLine("Current user not found");
            return RedirectToAction("Index", "Home");
        }
        
        var categories = await _budgetRepository.GetAllCategoriesByUserIdAsync(currentUser.Id);
        
        var top3Categories = categories
            .OrderByDescending(c => c.AllocatedAmount)
            .Take(3)
            .ToList();

        var today = DateTime.Today;
        var oneWeekBack = today.AddDays(-7);
        var fourWeeksAhead = today.AddDays(28);
        
        var upComingEvents = await _weddingRepository.GetAllByUserIdAsync(currentUser.Id);
        var filteredEvents = upComingEvents
            .Where(e =>
                e.StartTime != DateTime.MinValue &&
                e.EndTime != DateTime.MinValue &&
                e.StartTime.Date >= oneWeekBack &&
                e.StartTime.Date <= fourWeeksAhead)
            .OrderBy(e => e.StartTime)
            .ToList();
        
        var coupleDashboardViewModel = new CoupleDashboardViewModel
        {
            FullCoupleName = $"{currentUser.FirstName} & {coupleProfile.PartnerName}",
            WeddingDate = coupleProfile.WeddingDate,
            Checklists = await _checklistRepository.GetAllChecklistsByUserAsync(currentUser.Id),
            Guests = await _guestRepository.GetAllGuestUsingAppUserIdAsync(currentUser.Id),
            BudgetCategories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList(),
            BudgetSummary = new BudgetSummaryViewModel
            {
                TotalAllocated = currentUser.TotalAllocated,
                TotalSpent = currentUser.TotalUtilized,
                Top3Categories = top3Categories
            },
            WeddingEvents = filteredEvents
            
            
        };
        
        if (coupleProfile != null)
        {
            // Load planner requests
            var plannerRequests = await _userRepository.GetCoupleRequestsAsync(coupleProfile.Id);
            
            if (plannerRequests != null)
            {
                foreach (var request in plannerRequests)
                {
                    if (request?.Planner == null)
                        continue;
            
                    coupleDashboardViewModel.PlannerRequests.Add(new PlannerRequestViewModel
                    {
                        RequestId = request.Id,
                        PlannerId = request.PlannerId,
                        PlannerName = $"{request.Planner.FirstName} {request.Planner.LastName}",
                        RequestDate = request.RequestDate,
                        Status = request.Status
                    });
                }
            }
    
            // Check for accepted planner
            if (coupleProfile.AcceptedPlannerId != null)
            {
                var planner = await _userRepository.GetByIdAsync(coupleProfile.AcceptedPlannerId);
                if (planner != null)
                {
                    coupleDashboardViewModel.AcceptedPlannerName = $"{planner.FirstName} {planner.LastName}";
                }
            }
        }
        
        
        
        if (coupleProfile.PlannerId != null)
        {
            var planner = await _userRepository.GetByIdAsync(coupleProfile.PlannerId);
            if (planner != null)
            {
                coupleDashboardViewModel.PlannerName = $"{planner.FirstName} {planner.LastName}";
                coupleDashboardViewModel.PlannerRequestStatus = coupleProfile.PlannerRequestStatus;
            }
        }
        
        return View(coupleDashboardViewModel);
        
    }

    
    [HttpGet]
    public async Task<IActionResult> SelectPlanner()
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Get all approved planners
        var allUsers = await _userRepository.GetAllAsync();
        var planners = allUsers
            .Where(u => u.PlannerProfile != null && u.PlannerProfile.IsApproved)
            .ToList();
    
        // Get the couple's existing planner requests
        var coupleProfile = _userProfileRepository.CoupleProfile;
        var existingRequests = new List<string>();
    
        if (coupleProfile != null)
        {
            var plannerRequests = await _userRepository.GetCoupleRequestsAsync(coupleProfile.Id);
            if (plannerRequests != null)
            {
                existingRequests = plannerRequests
                    .Where(r => r.Status == PlannerRequestStatus.Requested || r.Status == PlannerRequestStatus.Accepted)
                    .Select(r => r.PlannerId)
                    .ToList();
            }
        }
    
        var viewModel = new AvailablePlannerViewModel
        {
            Planners = planners.Select(p => new PlannerViewModel
            {
                Id = p.Id,
                Name = $"{p.FirstName} {p.LastName}",
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                IsApproved = p.PlannerProfile.IsApproved,
                IsRequested = existingRequests.Contains(p.Id) || 
                              (coupleProfile?.PlannerId == p.Id && coupleProfile?.PlannerRequestStatus != PlannerRequestStatus.None)
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RequestPlanner(string plannerId)
    {
        var currentUser = _userProfileRepository.CurrentUser;
        var coupleProfile = _userProfileRepository.CoupleProfile;
    
        if (currentUser == null || coupleProfile == null)
        {
            return RedirectToAction("Index", "Home");
        }
    
        // Update the couple's planner and status
        coupleProfile.PlannerId = plannerId;
        coupleProfile.PlannerRequestStatus = PlannerRequestStatus.Requested;
    
        // Save changes
        await _userRepository.UpdateAsync(currentUser, currentUser.Id);
    
        TempData["Success"] = "Planner request sent successfully!";
        return RedirectToAction("Index");
    }

    #region Checklists

    //View Checklists (All)
    public async Task<IActionResult> Checklists()
    {
        
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser is null)
        {
            return RedirectToAction("Index", "Home");
        }
        var checklists = await _checklistRepository.GetAllChecklistsByUserAsync(currentUser.Id);
        return View("ViewChecklists", checklists);
    }
    
    [HttpGet]
    public IActionResult Create()
    { 
        
        var coupleProfile = _userProfileRepository.CoupleProfile;
        var currentUser = _userProfileRepository.CurrentUser;

        if (coupleProfile == null || currentUser == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View("CreateChecklist");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateChecklist(WeddingChecklistViewModel checklistViewModel)
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser is null) return RedirectToAction("Index", "Home");
        
        Checklist checklist = new Checklist()
        {

            AppUserId = currentUser.Id,
            AppUser = currentUser,
            Title = checklistViewModel.Title,
            CreatedDate = DateTime.Now,

        };
    
        await _checklistRepository.CreateChecklistAsync(checklist); // <- your repository method
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ViewChecklist(int id)
    {
        var checklist = await _checklistRepository.GetChecklistByIdAsync(id);
        return View("ChecklistDetail", checklist);
    }

    [HttpGet]
    public async Task<IActionResult> EditChecklist(int id)
    {
        var checklist = await _checklistRepository.GetChecklistByIdAsync(id);
        return View("EditChecklist", checklist);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditChecklist(WeddingChecklistViewModel checklistViewModel)
    {
        var listId = checklistViewModel.Id;
       
        
        var checklist = await _checklistRepository.GetChecklistByIdAsync(listId);
        if(checklist is null) return RedirectToAction("CheckLists", "Couple");
        
        checklist.Title = checklistViewModel.Title;
        await _checklistRepository.UpdateChecklistAsync(checklist);
        return RedirectToAction("Checklists");
        
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteChecklist(int id)
    {
        await _checklistRepository.DeleteChecklistAsync(id);
        return RedirectToAction("Checklists");
    }
    
    
    #endregion
    
    #region Items

    [HttpPost]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateItem(CreateItemViewModel createItemViewModel)
    {
        var checklist = await _checklistRepository.GetChecklistByIdAsync(createItemViewModel.ChecklistId);
        if (checklist is null)
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid)
        {
           

                checklist.Items =
                    await _itemRepository.GetAllChecklistItemsByChecklistIdAsync(createItemViewModel.ChecklistId);
                return View("ChecklistDetail", checklist);

        }

        var newItem = new ChecklistItem
        {
            WeddingChecklistId = createItemViewModel.ChecklistId,
            Checklist = checklist,
            Title = createItemViewModel.ItemName,
            Description = createItemViewModel.ItemDescription,
            DueDate = createItemViewModel.ItemDueDate,
            CheckInDate = createItemViewModel.ItemCheckDate,
            CreatedDate = DateTime.Now,
            IsCompleted = createItemViewModel.IsChecked
        };

        bool success = await _itemRepository.AddChecklistItemAsync(newItem);

        if (success)
        {
            ModelState.AddModelError("", "Failed to add item.");
           
            checklist.Items = await _itemRepository.GetAllChecklistItemsByChecklistIdAsync(checklist.Id);
            return View("ChecklistDetail", checklist);
        }
        checklist.Items = await _itemRepository.GetAllChecklistItemsByChecklistIdAsync(checklist.Id);
        return View("ChecklistDetail", checklist);
    }

    
    [HttpGet]
    public async Task<IActionResult> EditItem(int id)
    {
        var item = await _itemRepository.GetChecklistItemByIdAsync(id);
        return View("_EditItem", item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(ChecklistItem item)
    {
        var checklist = await _checklistRepository.GetChecklistByIdAsync(item.WeddingChecklistId); 
        if (checklist is null) return RedirectToAction("Index", "Home");
        
       
        
        if (!ModelState.IsValid)
        {
            checklist.Items = await _itemRepository.GetAllChecklistItemsByChecklistIdAsync(checklist.Id);
            return View("ChecklistDetail", checklist);
        }
        
        var itemToUpdate = new ChecklistItem
        {
            WeddingChecklistId = item.WeddingChecklistId,
            Checklist = checklist,
            Title = item.Title,
            Description = item.Description,
            DueDate = item.DueDate,
            IsCompleted = item.IsCompleted
   
        };
        
        if (item.IsCompleted)
        {
         itemToUpdate.CheckInDate = DateTime.Now; 
        }
        
        var result = await _itemRepository.UpdateChecklistItem(itemToUpdate, item.Id);
        checklist.Items = await _itemRepository.GetAllChecklistItemsByChecklistIdAsync(checklist.Id);
        if (!result)
        {
            ModelState.AddModelError("", "Failed to update item.");
            
            return View("ChecklistDetail", checklist);
        }
        
        return View("ChecklistDetail", checklist);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmItem(int id)
    {
        var item = await _itemRepository.GetChecklistItemByIdAsync(id);
        if (item == null) return NotFound();

        item.IsCompleted = !item.IsCompleted;
        await _itemRepository.UpdateConfirmStatus(item, id);

        return RedirectToAction("ViewChecklist", new { id = item.WeddingChecklistId });
    }


    [HttpPost, ActionName("DeleteItem")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _itemRepository.DeleteChecklistItem(id);
        return RedirectToAction("Checklists");
    }

    
    #endregion
    
    
    [HttpPost]
    public async Task<IActionResult> AddExpenseFromCouple(int categoryId, string description, decimal amount, DateTime date)
    {
        var expense = new Expense
        {
            BudgetCategoryId = categoryId,
            Description = description,
            Amount = amount,
            ExpenseDate = date
        };

        await _budgetRepository.AddExpenseAsync(expense, categoryId);

        TempData["Success"] = "Expense added successfully!";
        return RedirectToAction("Index"); // or "Dashboard" if you're redirecting there
    }
    
    
    [HttpGet]
    public async Task<IActionResult> AvailablePlanners()
    {
        var availablePlanners = await _userRepository.GetAllAvailablePlannersAsync();
    
        var viewModel = new AvailablePlannerViewModel()
        {
            Planners = availablePlanners.Select(p => new PlannerViewModel
            {
                Id = p.Id,
                Name = $"{p.FirstName} {p.LastName}",
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                IsApproved = p.PlannerProfile.IsApproved
            }).ToList()
        };
    
        return View(viewModel);
    }

    // [HttpPost]
    // public async Task<IActionResult> RequestPlanner(string plannerId, string message)
    // {
    //     var currentUser = _userProfileRepository.CurrentUser;
    //     if (currentUser == null)
    //     {
    //         return RedirectToAction("Index", "Home");
    //     }
    //
    //     await _userRepository.RequestPlannerAsync(currentUser.Id, plannerId, message);
    //
    //     TempData["Success"] = "Planner request sent successfully!";
    //     return RedirectToAction("Index");
    // }
    
    [HttpPost]
    public async Task<IActionResult> CancelPlannerRequest(int requestId)
    {
        await _userRepository.CancelPlannerRequestAsync(requestId);
        return RedirectToAction("Index");
    }
   
    // [HttpGet]
    // public async Task<IActionResult> AddItem()
    // {
    //     WeddingChecklistViewModel checklistViewModel = new WeddingChecklistViewModel();
    //    // checklistViewModel = await  _repository.
    //     return View("WeddingChecklistDetails", checklistViewModel);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> AddItem(ChecklistItem item)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         item.CreatedDate = DateTime.Now;
    //         await _repository.AddItemAsync(item);
    //         return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
    //     }
    //     
    //     item.UserId = _userManager.GetUserId(User);
    //     if (item.UserId == null)
    //     {
    //         return Unauthorized();
    //     }
    //     item.CreatedDate = DateTime.Now;
    //     return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> ToggleComplete(int itemId)
    // {
    //     var item = await _repository.GetItemByIdAsync(itemId);
    //     if (item == null) return NotFound();
    //
    //     item.IsCompleted = !item.IsCompleted;
    //     await _repository.UpdateItemAsync(item);
    //     return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> DeleteItem(int itemId)
    // {
    //     var item = await _repository.GetItemByIdAsync(itemId);
    //     if (item != null)
    //     {
    //         int checklistId = item.WeddingChecklistId;
    //         await _repository.DeleteItemAsync(itemId);
    //         return RedirectToAction(nameof(Details), new { id = checklistId });
    //     }
    //     return NotFound();
    // }
    //
    // public async Task<IActionResult> Details(int id)
    // {
    //     var checklist = await _repository.GetChecklistByIdAsync(id);
    //     if (checklist == null) return NotFound();
    //
    //     var vm = new WeddingChecklistViewModel
    //     {
    //          Id = checklist.Id,
    //         Title = checklist.Title,
    //         Items = checklist.Items?.ToList() ?? new List<ChecklistItem>()
    //     };
    //
    //     return View("WeddingChecklistDetails",vm);
    // }
    }
    

}
