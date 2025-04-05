using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DreamDay.Data.Enums;

namespace DreamDay.Models;

public class Guest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Guest Name")]
    public required string GuestName { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(15)]
    [Phone]
    public string? Phone { get; set; }

    [Display(Name = "RSVP Status")]
    public RsvpStatusTypes RsvpStatus { get; set; }

    [Display(Name = "Meal Preference")]
    public MealPreferenceTypes MealPreference { get; set; }

    [ForeignKey("AppUser")]
    public required string AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    [ForeignKey("CoupleProfile")]
    public int? CoupleProfileId { get; set; }
    public CoupleProfile? CoupleProfile { get; set; }
}