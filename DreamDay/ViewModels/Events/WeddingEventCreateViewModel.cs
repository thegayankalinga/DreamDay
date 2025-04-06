using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.ViewModels.Events;

public class WeddingEventCreateViewModel
{
    public int Id { get; set; }

    [Required] 
    [MaxLength(100)] 
    public required string Title { get; set; } = "";

    [MaxLength(500)]  // Increased maximum length for descriptions
    public string? Description { get; set; }

    [Required]
    [DataType(DataType.DateTime)]  // Explicitly define data type
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.DateTime)]  // Explicitly define data type
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);

    public string? Location { get; set; }

    public int? ChecklistId { get; set; } // Made nullable to allow no checklist selection

    public int? VenueId { get; set; }

    // Added for dropdown binding
    public SelectList? AvailableChecklists { get; set; }
    
    public SelectList? AvailableVenues { get; set; }

    public bool IsEditMode => Id > 0;
}