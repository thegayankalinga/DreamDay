using DreamDay.Models;

namespace DreamDay.ViewModels.Planner;

public class WeddingDetailsViewModel
{
    // Basic wedding information
    public int CoupleProfileId { get; set; }
    public required string CoupleName { get; set; }
    public string? CoupleUserId { get; set; }
    public DateOnly WeddingDate { get; set; }
    public int DaysUntilWedding { get; set; }
    public string? VenueName { get; set; }
    public string? Status { get; set; }
    public string? StatusBadgeColor { get; set; }

    // Checklist information
    public int CompletedChecklistItems { get; set; }
    public int TotalChecklistItems { get; set; }
    public int ChecklistProgressPercentage => TotalChecklistItems > 0 
        ? (int)((float)CompletedChecklistItems / TotalChecklistItems * 100) 
        : 0;
    public List<ChecklistItem>? RecentChecklistItems { get; set; }

    // Budget information
    public decimal Budget { get; set; }
    public decimal BudgetUtilized { get; set; }
    public int BudgetUtilizationPercentage => Budget > 0 
        ? (int)(BudgetUtilized / Budget * 100) 
        : 0;
    public List<BudgetCategory>? BudgetCategories { get; set; }

    // Guest information
    public int TotalGuests { get; set; }
    public int AttendingGuests { get; set; }
    public int DeclinedGuests { get; set; }
    public int PendingGuests { get; set; }

    // Timeline information
    public List<WeddingEvent>? WeddingEvents { get; set; }
}