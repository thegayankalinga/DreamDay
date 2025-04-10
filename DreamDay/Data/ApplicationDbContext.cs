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
        
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(450); // 450 is common for string IDs
        });
        
        //wedding event
        modelBuilder.Entity<WeddingEventVendor>()
            .HasKey(tv => new { tv.TimelineEventId, tv.VendorId });

        modelBuilder.Entity<WeddingEventVendor>()
            .HasOne(tv => tv.WeddingEvent)
            .WithMany(te => te.WeddingEventVendors)
            .HasForeignKey(tv => tv.TimelineEventId);

        modelBuilder.Entity<WeddingEventVendor>()
            .HasOne(tv => tv.Vendor)
            .WithMany(v => v.WeddingEventVendors)
            .HasForeignKey(tv => tv.VendorId);
        
        // Checklist
        modelBuilder.Entity<Checklist>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Checklist)
            .HasForeignKey(i => i.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChecklistItem>()
            .HasOne(i => i.Checklist)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.WeddingChecklistId)
            .OnDelete(DeleteBehavior.Cascade); // Allow cascade here

        modelBuilder.Entity<ChecklistItem>()
            .HasOne<AppUser>() // or your actual User model
            .WithMany()
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
        
        // Configure the MessageThread entity
        modelBuilder.Entity<MessageThread>()
            .HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<MessageThread>()
            .HasOne(t => t.Recipient)
            .WithMany()
            .HasForeignKey(t => t.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure the Message entity
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Thread)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.MessageThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    
    //You can  get a property by typing "prop"
    
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet <PlannerProfile> PlannerProfiles { get; set; }
    public DbSet <CoupleProfile> CoupleProfiles { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }
    public DbSet<WeddingEvent> WeddingEvents { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<WeddingEventVendor> WeddingEventVendors { get; set; }
    public DbSet<MessageThread> MessageThreads { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PlannerRequest> PlannerRequests { get; set; }

    

}