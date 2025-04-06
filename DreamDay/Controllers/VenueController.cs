using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class VenueController : Controller
{
    private readonly IVenueRepository _venueRepository;

    public VenueController(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }
    
    // GET: Venue (Public view)
    public async Task<IActionResult> Index()
    {
        var venues = await _venueRepository.GetAllVenuesAsync();
        return View(venues);
    }
    
    // GET: Venue/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue == null)
        {
            return NotFound();
        }
        return View(venue);
    }

    #region Admin Functions
    
    // GET: Admin/Venues
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("Admin/Venues")]
    public async Task<IActionResult> AdminVenues()
    {
        var venues = await _venueRepository.GetAllVenuesAsync();
        return View("~/Views/Admin/Venues.cshtml", venues);
    }
    
    // POST: Venue/Create
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue)
    {
        if (ModelState.IsValid)
        {
            await _venueRepository.AddAsync(venue);
            return RedirectToAction("AdminVenues", "Venue");
        }
        return RedirectToAction("AdminVenues", "Venue");
    }

    // POST: Venue/Edit/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue venue)
    {
        if (id != venue.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _venueRepository.UpdateAsync(venue, id);
            return RedirectToAction("AdminVenues", "Venue");
        }
        return RedirectToAction("AdminVenues", "Venue");
    }

    // POST: Venue/Delete/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _venueRepository.DeleteAsync(id);
        return RedirectToAction("AdminVenues", "Venue");
    }
    
    #endregion
}