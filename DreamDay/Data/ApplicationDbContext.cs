using DreamDay.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Data;

// public class ApplicationDbContext: DbContext (to be used with EntityFramework
public class ApplicationDbContext: IdentityDbContext<AppUser> //to be used with Identity.entityframework
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    //You can  get a property by typing "prop"
    
    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet <PlannerProfile> PlannerProfiles { get; set; }
    public DbSet <CoupleProfile> CoupleProfiles { get; set; }
}