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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeddingChecklistItem>()
            .HasOne(i => i.WeddingChecklist)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.WeddingChecklistId)
            .OnDelete(DeleteBehavior.Cascade); // Allow cascade here

        modelBuilder.Entity<WeddingChecklistItem>()
            .HasOne<AppUser>() // or your actual User model
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade to avoid conflict
        
        // One-to-One: AppUser (as couple) <--> CoupleProfile
        modelBuilder.Entity<AppUser>()
            .HasOne(a => a.CoupleProfile)
            .WithOne(cp => cp.AppUser)
            .HasForeignKey<CoupleProfile>(cp => cp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: AppUser (as planner) --> many CoupleProfiles
        modelBuilder.Entity<AppUser>()
            .HasMany(a => a.AssignedCoupleProfiles)
            .WithOne(cp => cp.Planner)
            .HasForeignKey(cp => cp.PlannerId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid multiple cascade paths
    }

    
    //You can  get a property by typing "prop"
    
    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet <PlannerProfile> PlannerProfiles { get; set; }
    public DbSet <CoupleProfile> CoupleProfiles { get; set; }
    
    public DbSet<WeddingChecklist> WeddingChecklists { get; set; }
    public DbSet<WeddingChecklistItem> WeddingChecklistItems { get; set; }

}