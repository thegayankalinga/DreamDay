using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DreamDay.Data.Enums;

namespace DreamDay.Models;

public class PlannerRequest
{
    public int Id { get; set; }
    
    [ForeignKey("CoupleProfile")]
    public int? CoupleProfileId { get; set; }
    public CoupleProfile? CoupleProfile { get; set; }
    
    [MaxLength(450)]
    [ForeignKey("Planner")]
    public string? PlannerId { get; set; }
    public AppUser? Planner { get; set; }
    
    public string? Message { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.Now;
    
    public PlannerRequestStatus Status { get; set; } = PlannerRequestStatus.Requested;
}