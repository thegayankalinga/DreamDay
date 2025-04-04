using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class Checklist
{
    [Key]
    public int Id { get; init; }
    
    [ForeignKey("AppUser")]
    public required string AppUserId { get; set; }
    public required AppUser AppUser { get; set; }
    
    [MaxLength(100)]
    public required string Title { get; set; }
    public DateTime CreatedDate { get; init; } = DateTime.Now;
    
    public ICollection<ChecklistItem>? Items { get; set; }
}