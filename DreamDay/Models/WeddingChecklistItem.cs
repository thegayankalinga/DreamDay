using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class WeddingChecklistItem
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("AppUser")]
    public string? UserId { get; set; }
    public AppUser AppUser { get; set; }
    
    [ForeignKey("WeddingChecklist")]
    public int WeddingChecklistId { get; set; }
    public WeddingChecklist WeddingChecklist { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public bool IsCompleted { get; set; }
}