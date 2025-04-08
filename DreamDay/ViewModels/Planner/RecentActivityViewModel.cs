namespace DreamDay.ViewModels.Planner;

public class RecentActivityViewModel
{
    public string? Description { get; set; }
    public DateTime ActivityTime { get; set; }
    public string? CoupleName { get; set; }
    public int CoupleProfileId { get; set; }
    public string? ActivityType { get; set; } // "Message", "Checklist", "Guest", "Budget",
}