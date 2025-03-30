using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class CoupleProfile
{
    public int Id { get; set; }

    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }      // Points to the couple user
    public AppUser? AppUser { get; set; }

    [ForeignKey("Planner")]
    public string? PlannerId { get; set; }     // Points to the assigned planner
    public AppUser? Planner { get; set; }      // Also an AppUser, with Planner role

    public DateOnly WeddingDate { get; set; }
    public string PartnerName { get; set; }
}
