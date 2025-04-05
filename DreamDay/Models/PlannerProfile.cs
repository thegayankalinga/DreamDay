using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class PlannerProfile
{
    [Key]
    public int Id { get; set; }
    [MaxLength(450)]
    [ForeignKey("AppUser")]
    public required string AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public bool IsApproved { get; set; }
    
    public ICollection<CoupleProfile>? AssignedCoupleProfiles { get; set; }
}