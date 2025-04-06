using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.ViewModels.Events;

public class WeddingEventCreateViewModel
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public string? Location { get; set; }

    [Required]
    public int ChecklistId { get; set; } // Must now be selected from user’s checklists

    public int? VenueId { get; set; }

    // Added for dropdown binding
    public SelectList? AvailableChecklists { get; set; }
}