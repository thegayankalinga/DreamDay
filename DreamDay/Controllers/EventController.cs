﻿
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.Controllers;

public class EventController : Controller
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IWeddingEventRepository _weddingEventRepository;
    // private readonly IVendorRepository _vendorRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IChecklistRepository _checklistRepository;

    public EventController(
        IUserProfileRepository userProfileRepository, 
        IWeddingEventRepository repository,
        IVenueRepository venueRepository,
        IChecklistRepository checklistRepository
        )
    {
        _userProfileRepository = userProfileRepository;
        _weddingEventRepository = repository;
        _venueRepository = venueRepository;
        _checklistRepository = checklistRepository;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var userId = _userProfileRepository.CurrentUser?.Id;
        if (userId == null) return RedirectToAction("Login", "Account");


        var events = await _weddingEventRepository.GetAllByUserIdAsync(userId);

        var venues = await _venueRepository.GetAllVenuesAsync();
        
        var checklists = await _checklistRepository.GetAllChecklistsByUserAsync(userId);

        

        var viewModel = new WeddingEventDashboardViewModel
        {
            Events = events,
            AvailableVenues = new SelectList(venues, "Id", "Name"),
            AvailableChecklists = new SelectList(checklists, "Id", "Title")
        };

        return View(viewModel);
    }
    
    //Events
    [HttpPost]
    public async Task<IActionResult> CreateEvent(WeddingEventDashboardViewModel model)
    {
        var userId = _userProfileRepository.CurrentUser?.Id;
        if (userId == null) return RedirectToAction("Login", "Account");
        
        var newEvent = new WeddingEvent
        {
            Title = model.NewEvent.Title,
            Description = model.NewEvent.Description,
            StartTime = model.NewEvent.StartTime,
            EndTime = model.NewEvent.EndTime,
            Location = model.NewEvent.Location,
            ChecklistId = model.NewEvent.ChecklistId,
            VenueId = model.NewEvent.VenueId,
            UserId = userId

        };
        await _weddingEventRepository.AddAsync(newEvent, userId);
        
        return RedirectToAction("Index");
    }
}