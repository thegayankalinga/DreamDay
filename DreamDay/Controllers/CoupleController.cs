using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers.Dashboard
{
    [Authorize(Policy = "RequireCoupleRole")]
    public class CoupleController : Controller
    {
        
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly IItemRepository _itemRepository;

    public CoupleController(
        IUserProfileRepository userProfileRepository, 
        IChecklistRepository checklistRepository, 
        IItemRepository itemRepository)
    {
        _userProfileRepository = userProfileRepository;
        _checklistRepository = checklistRepository;
        _itemRepository = itemRepository;
    }
    
    [Authorize(Policy = "RequireCoupleRole")]
    public async Task<IActionResult> Index()
    {
        var coupleProfile = _userProfileRepository.CoupleProfile;
        // var currentUserRole = _userProfileRepository.UserType;
        var currentUser = _userProfileRepository.CurrentUser;
        if (coupleProfile == null)
        {
            Console.WriteLine("Couple profile not found");
            return RedirectToAction("Index", "Home");
        }

        if (currentUser is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var coupleDashboardViewModel = new CoupleDashboardViewModel
        {
            FullCoupleName = $"{currentUser.FirstName} & {coupleProfile.PartnerName}",
            WeddingDate = coupleProfile.WeddingDate,
            Checklists = await _checklistRepository.GetAllChecklistsByUserAsync(currentUser.Id)
        };
        
        return View(coupleDashboardViewModel);
        
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var coupleProfile = _userProfileRepository.CoupleProfile;
        // var user = await _userManager.GetUserAsync(User);
        // var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
        //
        // if (user == null)
        // {
        //     return Unauthorized();
        // }
        //
        // // var user = await _userManager.Users
        // //     .Include(u => u.CoupleProfile)
        // //     .ThenInclude(cp => cp.WeddingChecklists)
        // //     .FirstOrDefaultAsync(u => u.Id == userId);
        //     
        // var coupleProfile = await _context.CoupleProfiles.Include(cp => cp.AppUser).FirstOrDefaultAsync(cp => cp.AppUserId == user.Id);
        // var role = await _userManager.GetRolesAsync(user);
        // if (coupleProfile == null && !role.Contains(UserRoles.Couple) )
        // {
        //     Console.WriteLine("Couple profile not found");
        //     return RedirectToAction("Create", "Couple");
        // }

            
        var vm = new WeddingChecklistViewModel()
        {
                
            CoupleFullName = "",
            CoupleProfile = coupleProfile,
            
        };
        
        return View("CreateChecklist", vm);
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateChecklist(WeddingChecklistViewModel checklistViewModel)
    // {
    //     var user = await _userManager.Users
    //         .Include(u => u.CoupleProfile)
    //         .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
    //
    //     if (user?.CoupleProfile == null)
    //     {
    //         Console.WriteLine("Couple profile not found");
    //         return BadRequest("Couple profile not found.");
    //     }
    //
    //     Checklist checklist = new Checklist()
    //     {
    //         AppUserId = user.Id,
    //         Title = checklistViewModel.Title,
    //         CreatedDate = checklistViewModel.CreatedAt,
    //         
    //
    //     };
    //
    //     await _repository.CreateChecklistAsync(checklist); // <- your repository method
    //     return RedirectToAction("Index");
    // }

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
