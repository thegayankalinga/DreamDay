
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
            NewEvent = new WeddingEventCreateViewModel
            {
                Title = ""
            },
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
        
        // Validation
        if (string.IsNullOrWhiteSpace(model.NewEvent.Title))
        {
            // Could add ModelState errors here and return to view with validation errors
            return RedirectToAction("Index");
        }
        
        var eventToSave = new WeddingEvent
        {
            Id = model.NewEvent.Id, // This will be 0 for new events, or the existing ID for updates
            Title = model.NewEvent.Title,
            Description = model.NewEvent.Description,
            StartTime = model.NewEvent.StartTime,
            EndTime = model.NewEvent.EndTime,
            Location = model.NewEvent.Location,
            ChecklistId = model.NewEvent.ChecklistId,
            VenueId = model.NewEvent.VenueId,
            UserId = userId
        };

        if (model.NewEvent.Id == 0)
        {
            // Create new event
            await _weddingEventRepository.AddAsync(eventToSave, userId);
        }
        else
        {
            // Update existing event
            var existingEvent = await _weddingEventRepository.GetByIdAsync(model.NewEvent.Id);
            if (existingEvent == null || existingEvent.UserId != userId)
            {
                // Event not found or doesn't belong to current user
                return RedirectToAction("Index");
            }
            
            await _weddingEventRepository.UpdateAsync(eventToSave, model.NewEvent.Id);
        }
        
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetEventJson(int id)
    {
        var userId = _userProfileRepository.CurrentUser?.Id;
        if (userId == null) return Unauthorized();
        
        var evt = await _weddingEventRepository.GetByIdAsync(id);
        if (evt == null) return NotFound();
        
        // Verify that the event belongs to the current user
        if (evt.UserId != userId) return Unauthorized();

        // Format the datetime values carefully to ensure browser compatibility
        // Format: yyyy-MM-ddTHH:mm (no seconds, no milliseconds)
        string startTime = evt.StartTime.ToString("yyyy-MM-ddTHH:mm");
        string endTime = evt.EndTime.ToString("yyyy-MM-ddTHH:mm");

        return Json(new
        {
            evt.Id,
            evt.Title,
            evt.Description,
            StartTime = startTime,
            EndTime = endTime,
            evt.VenueId,
            evt.ChecklistId
        });
    }
}