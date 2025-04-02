using DreamDay.Models;

namespace DreamDay.ViewModels;

public class CoupleDashboardViewModel
{
    public string FullCoupleName { get; set; }
    public DateOnly WeddingDate { get; set; }
    public IEnumerable<WeddingChecklist> WeddingChecklists { get; set; }

    // Future features placeholders
    public bool IsGuestListReady => false;
    public bool IsBudgetTrackerReady => false;
    public bool IsTimelineReady => false;
}