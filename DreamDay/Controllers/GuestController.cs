using DreamDay.Data.Enums;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.Controllers;

[Authorize(Policy = "RequireCoupleRole")]
public class GuestController : Controller
{
    private readonly IGuestRepository _guestRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public GuestController(IGuestRepository guestRepository, IUserProfileRepository userProfileRepository)
    {
        _guestRepository = guestRepository;
        _userProfileRepository = userProfileRepository;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var currentUser = _userProfileRepository.CurrentUser;
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var guests = await _guestRepository.GetAllGuestUsingAppUserIdAsync(currentUser.Id);
        return View(guests);
    }
    
    //Get for Guest Creation
    [HttpGet]
    public async Task<IActionResult> CreateGuest()
    {
        var currentUser = _userProfileRepository.CurrentUser;
        var coupleProfile = _userProfileRepository.CoupleProfile;
        
        if(currentUser == null) return RedirectToAction("Login", "Account");
        if(coupleProfile == null) return RedirectToAction("Login", "Account");

        Guest guest = new Guest
        {
            GuestName = "",
            Email = "",
            Phone = "",
            AppUserId = currentUser.Id,
            AppUser = currentUser,
            CoupleProfileId = coupleProfile.Id,
            CoupleProfile = coupleProfile,
        };
        
        ViewBag.RsvpOptions = new SelectList(Enum.GetValues(typeof(RsvpStatusTypes)));
        ViewBag.MealOptions = new SelectList(Enum.GetValues(typeof(MealPreferenceTypes)));

        
        return View("CreateGuest", guest);
        
        
    }
    
    //Save Guest
    [HttpPost]
    public async Task<IActionResult> CreateGuest(Guest guest)
    {
        var currentUser = _userProfileRepository.CurrentUser;
        var coupleProfile = _userProfileRepository.CoupleProfile;
        if (currentUser == null) return RedirectToAction("Login", "Account");
        if (coupleProfile == null) return RedirectToAction("Login", "Account");
        
        guest.AppUserId = currentUser.Id;
        guest.AppUser = currentUser;
        guest.CoupleProfileId = coupleProfile.Id;
        guest.CoupleProfile = coupleProfile;
        
        var result = await _guestRepository.AddGuestAsync(guest);
        if (!result)
        {
            return View("CreateGuest", guest);
        }
        return RedirectToAction("Index");
    }
    
    
    //Get for edit
    [HttpGet]
    public async Task<IActionResult> EditGuest(int id)
    {
       
        var guestToEdit = await _guestRepository.GetByGuestIdAsync(id);
    

        if (guestToEdit == null) return View("Index");
        Guest guest = new Guest
        {
            Id = guestToEdit.Id,
            GuestName = guestToEdit.GuestName,
            Email = guestToEdit.Email,
            Phone = guestToEdit.Phone,
            RsvpStatus = guestToEdit.RsvpStatus,
            MealPreference = guestToEdit.MealPreference,
            CoupleProfileId = guestToEdit.CoupleProfileId,
            CoupleProfile = guestToEdit.CoupleProfile,
            AppUserId = guestToEdit.AppUserId,
            AppUser = guestToEdit.AppUser,

        };
        
        return View("EditGuest", guest);
    }
    
    //Update Guest (Post)
    [HttpPost]
    public async Task<IActionResult> EditGuest(Guest guest)
    {
        var result = await _guestRepository.UpdateGuestAsync(guest, guest.Id);

        return RedirectToAction("index");
    }

    [HttpPost, ActionName("DeleteGuest")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        await _guestRepository.DeleteGuestAsync(id);
        return RedirectToAction("index");
    }
}