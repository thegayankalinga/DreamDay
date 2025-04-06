using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    // GET
    public async  Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        
        
        
        return View(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index"); // You can later return with validation errors

        var user = await _userRepository.GetByIdAsync(model.Id);
        if (user == null)
            return NotFound();

        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.UserName = model.Email;

        // Update Couple Profile
        if (model.Role == UserRoles.Couple)
        {
            if (string.IsNullOrWhiteSpace(model.PartnerName) || model.WeddingDate == null)
            {
                TempData["Error"] = "Both partner name and wedding date are required for couple profile.";
                return RedirectToAction("Index");
            }

            if (user.CoupleProfile == null)
            {
                user.CoupleProfile = new CoupleProfile
                {
                    AppUserId = user.Id,
                    PartnerName = model.PartnerName,
                    WeddingDate = (DateOnly)model.WeddingDate,
                };
            }

            user.CoupleProfile.PartnerName = model.PartnerName ?? user.CoupleProfile.PartnerName;
            user.CoupleProfile.WeddingDate = model.WeddingDate ?? user.CoupleProfile.WeddingDate;
            user.CoupleProfile.PlannerId = model.PlannerId;
        }

        // Update Planner Profile
        if (model.Role == UserRoles.Planner)
        {
            if (user.PlannerProfile == null)
            {
                user.PlannerProfile = new PlannerProfile
                {
                    AppUserId = user.Id,
                };
            }

            user.PlannerProfile.IsApproved = model.IsApproved;
        }

        await _userRepository.UpdateAsync(user, user.Id);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        //TODO: Delete Logic needs to delete everything based on the user
        await _userRepository.DeleteAsync(id);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> ToggleApproval(string userId)
    {
        await _userRepository.TogglePlannerActivationAsync(userId);
        return RedirectToAction("Index");
    }
}