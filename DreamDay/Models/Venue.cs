using System.ComponentModel.DataAnnotations;

namespace DreamDay.Models;

public class Venue
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    public ICollection<WeddingEvent>? WeddingEvents { get; set; } 
    public List<CoupleProfile>? CoupleProfiles { get; set; }
}