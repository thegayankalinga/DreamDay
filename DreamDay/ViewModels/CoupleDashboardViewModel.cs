using DreamDay.Models;

namespace DreamDay.ViewModels;

public class CoupleDashboardViewModel
{
    public required string FullCoupleName { get; set; }
    public DateOnly WeddingDate { get; set; }
    public List<Checklist> Checklists { get; set; } = new List<Checklist>();

    // Future features placeholders
    public bool IsGuestListReady => false;
    public bool IsBudgetTrackerReady => false;
    public bool IsTimelineReady => false;
}