
using DreamDay.Interfaces;
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

    public EventController(
        IUserProfileRepository userProfileRepository, 
        IWeddingEventRepository repository,
        IVenueRepository venueRepository
        )
    {
        _userProfileRepository = userProfileRepository;
        _weddingEventRepository = repository;
        _venueRepository = venueRepository;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var userId = _userProfileRepository.CurrentUser?.Id;
        if (userId == null) return RedirectToAction("Login", "Account");


        var events = await _weddingEventRepository.GetAllByUserIdAsync(userId);

        var venues = await _venueRepository.GetAllVenuesAsync();

        var viewModel = new WeddingEventDashboardViewModel
        {
            Events = events,
            AvailableVenues = new SelectList(venues, "Id", "Name")
        };

        return View(viewModel);
    }
}