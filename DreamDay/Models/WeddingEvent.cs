using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class WeddingEvent
{
    [Key] public int Id { get; set; }

    [MaxLength(100)] public required string Title { get; set; }

    [MaxLength(200)] public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [MaxLength(100)] public string? Location { get; set; }

    [ForeignKey("Checklist")] public int ChecklistId { get; set; }
    public Checklist? Checklist { get; set; }
    
    [ForeignKey("AppUser")] 
    public string? UserId { get; set; }
    public AppUser? User { get; set; } // To link to a specific couple 
    
    public ICollection<WeddingEventVendor>? WeddingEventVendors { get; set; }
}