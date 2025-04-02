using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class WeddingChecklist
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("AppUser")]
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    
    public string Title { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public ICollection<WeddingChecklistItem> Items { get; set; }
}