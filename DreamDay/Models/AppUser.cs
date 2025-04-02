using Microsoft.AspNetCore.Identity;

namespace DreamDay.Models;

public class AppUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public CoupleProfile? CoupleProfile { get; set; }
    
    public ICollection<CoupleProfile>? AssignedCoupleProfiles { get; set; }
}