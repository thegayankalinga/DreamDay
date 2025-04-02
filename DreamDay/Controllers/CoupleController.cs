using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers.Dashboard
{
    public class CoupleController : Controller
    {
        private readonly IWeddingChecklistRepository _repository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

    public CoupleController(IWeddingChecklistRepository repository, UserManager<AppUser> userManager, ApplicationDbContext context)
    {
        _repository = repository;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
     
        var user = await _userManager.GetUserAsync(User);
        var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
        
        if (user == null)
        {
            return Unauthorized();
        }

            // var user = await _userManager.Users
            //     .Include(u => u.CoupleProfile)
            //     .ThenInclude(cp => cp.WeddingChecklists)
            //     .FirstOrDefaultAsync(u => u.Id == userId);
            
            var coupleProfile = await _context.CoupleProfiles.Include(cp => cp.AppUser).FirstOrDefaultAsync(cp => cp.AppUserId == user.Id);
            var role = await _userManager.GetRolesAsync(user);
            if (coupleProfile == null && !role.Contains(UserRoles.Couple) )
            {
                Console.WriteLine("Couple profile not found");
                return RedirectToAction("Create", "CoupleProfile");
            }

            
            var vm = new CoupleDashboardViewModel
            {
                
                FullCoupleName = $"{coupleProfile.PartnerName} & {appUser.FirstName}",
                WeddingDate = coupleProfile.WeddingDate,
                WeddingChecklists = coupleProfile.WeddingChecklists ?? new List<WeddingChecklist>()
            };

            return View(vm);
        
    }

    public IActionResult Create()
    {
        return View("CreateChecklist");
    }

    [HttpPost]
    public async Task<IActionResult> CreateChecklist(WeddingChecklistViewModel checklistViewModel)
    {
        var user = await _userManager.Users
            .Include(u => u.CoupleProfile)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user?.CoupleProfile == null)
        {
            Console.WriteLine("Couple profile not found");
            return BadRequest("Couple profile not found.");
        }

        WeddingChecklist checklist = new WeddingChecklist()
        {
            AppUserId = user.Id,
            Title = checklistViewModel.Title,
            CreatedDate = checklistViewModel.CreatedAt,

        };

        await _repository.CreateChecklistAsync(checklist); // <- your repository method
        return RedirectToAction("Index");
    }

   

    [HttpPost]
    public async Task<IActionResult> AddItem(WeddingChecklistItem item)
    {
        if (ModelState.IsValid)
        {
            item.CreatedDate = DateTime.Now;
            await _repository.AddItemAsync(item);
            return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
        }
        
        item.UserId = _userManager.GetUserId(User);
        if (item.UserId == null)
        {
            return Unauthorized();
        }
        item.CreatedDate = DateTime.Now;
        return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleComplete(int itemId)
    {
        var item = await _repository.GetItemByIdAsync(itemId);
        if (item == null) return NotFound();

        item.IsCompleted = !item.IsCompleted;
        await _repository.UpdateItemAsync(item);
        return RedirectToAction(nameof(Details), new { id = item.WeddingChecklistId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(int itemId)
    {
        var item = await _repository.GetItemByIdAsync(itemId);
        if (item != null)
        {
            int checklistId = item.WeddingChecklistId;
            await _repository.DeleteItemAsync(itemId);
            return RedirectToAction(nameof(Details), new { id = checklistId });
        }
        return NotFound();
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var checklist = await _repository.GetChecklistByIdAsync(id);
        if (checklist == null) return NotFound();

        var vm = new WeddingChecklistViewModel
        {
             Id = checklist.Id,
            Title = checklist.Title,
            Items = checklist.Items?.ToList() ?? new List<WeddingChecklistItem>()
        };

        return View("WeddingChecklistDetails",vm);
    }
    }
    

}
