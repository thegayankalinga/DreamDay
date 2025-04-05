using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DreamDay.Models;

public class AppUser: IdentityUser
{
    [MaxLength(100)]
    public required string FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    public CoupleProfile? CoupleProfile { get; set; }
    
    public ICollection<CoupleProfile>? AssignedCoupleProfiles { get; set; }
    public ICollection<Checklist>? AssignedChecklists { get; set; }
    public ICollection<Guest>? AssignedGuests { get; set; }
    public ICollection<BudgetCategory>? AssignedBudgetCategories { get; set; }
}