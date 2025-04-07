using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DreamDay.Models;

public class AppUser: IdentityUser
{
    [MaxLength(100)]
    public required string FirstName { get; set; }
    [MaxLength(100)]
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    //One to one couple profile
    public CoupleProfile? CoupleProfile { get; set; }
    
   
    
    //one to one planner profile
    public PlannerProfile? PlannerProfile { get; set; }
    public ICollection<CoupleProfile>? AssignedCoupleProfiles { get; set; } //if he is a planner
    public ICollection<Checklist>? AssignedChecklists { get; set; } //if he is a couple
    public ICollection<Guest>? AssignedGuests { get; set; } //if he is a couple
    public ICollection<BudgetCategory>? AssignedBudgetCategories { get; set; } // if he is a couple

    #region NotMapped
    [NotMapped] 
    public decimal? TotalAllocated => AssignedBudgetCategories?.Sum(category => category.AllocatedAmount) ?? 0;
    [NotMapped]
    public decimal TotalUtilized =>
        AssignedBudgetCategories?.Sum(category =>
            category.Expenses?.Sum(ex => ex.Amount) ?? 0
        ) ?? 0;
    #endregion


}   