using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DreamDay.Data.Enums;

namespace DreamDay.Models;

public class CoupleProfile
{
    public int Id { get; set; }
    public DateOnly WeddingDate { get; set; }
    [MaxLength(100)]
    public required string PartnerName { get; set; }
    
    #region Navigations & Relationships
    
    [ForeignKey("Venue")]
    public int? VenueId { get; set; }
    public Venue? Venue { get; set; }

    [MaxLength(450)]
    [ForeignKey("AppUser")]
    public required string AppUserId { get; set; }      // Points to the couple user
    public AppUser? AppUser { get; set; }
    
    [ForeignKey("Planner")]
    [MaxLength(450)]
    public string? PlannerId { get; set; }     // Points to the assigned planner
    public AppUser? Planner { get; set; }      // Also an AppUser, with Planner role
    
    public ICollection<PlannerRequest> PlannerRequests { get; set; } = new List<PlannerRequest>();
    public PlannerRequestStatus PlannerRequestStatus { get; set; } = PlannerRequestStatus.None;
// This property will point to the accepted planner (if any)
    [ForeignKey("AcceptedPlanner")]
    [MaxLength(450)]
    public string? AcceptedPlannerId { get; set; }
    public AppUser? AcceptedPlanner { get; set; }
    
    public ICollection<Checklist>? WeddingChecklists { get; set; }
    public List<Guest>? Guests { get; set; }

    #endregion
}
