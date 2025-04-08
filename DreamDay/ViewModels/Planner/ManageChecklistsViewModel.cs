using DreamDay.Models;

namespace DreamDay.ViewModels.Planner;

public class ManageChecklistsViewModel
{
    // Basic wedding information
    public int CoupleProfileId { get; set; }
    public required string CoupleName { get; set; }
    public DateOnly WeddingDate { get; set; }
    public int DaysUntilWedding { get; set; }
    public string? Status { get; set; }
    public string? StatusBadgeColor { get; set; }

    // Checklist information
    public int CompletedChecklistItems { get; set; }
    public int TotalChecklistItems { get; set; }
    public int ChecklistProgressPercentage => TotalChecklistItems > 0 
        ? (int)((float)CompletedChecklistItems / TotalChecklistItems * 100) 
        : 0;
        
    // All checklists for this couple
    public List<Checklist> Checklists { get; set; } = new List<Checklist>();
        
    // For planner permissions
    public bool CanEditChecklists { get; set; } = true;
}