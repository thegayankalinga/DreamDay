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
        
        var vm = new WeddingChecklistViewModel()
        {
                
            CoupleFullName = "",
            CoupleProfile = coupleProfile,
            
        };
        
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
        var result = await _checklistRepository.DeleteChecklistAsync(id);
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
        var result = await _itemRepository.DeleteChecklistItem(id);
        return RedirectToAction("Checklists");
    }

    
    #endregion
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
