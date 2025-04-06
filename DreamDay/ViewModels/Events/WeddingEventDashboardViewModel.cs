using DreamDay.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.ViewModels.Events;

public class WeddingEventDashboardViewModel
{
    public List<WeddingEvent>? Events { get; set; }

    public WeddingEventCreateViewModel NewEvent { get; set; } = new WeddingEventCreateViewModel();

    public SelectList? AvailableVenues { get; set; }
    public SelectList? AvailableChecklists { get; set; }
}