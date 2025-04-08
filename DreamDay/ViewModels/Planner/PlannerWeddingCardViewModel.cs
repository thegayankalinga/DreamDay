namespace DreamDay.ViewModels.Planner;

public class PlannerWeddingCardViewModel
{
    public int CoupleProfileId { get; set; }
    public required string CoupleName { get; set; }
    public DateOnly WeddingDate { get; set; }
    public string? VenueName { get; set; }
    public int CompletedChecklistItems { get; set; }
    public int TotalChecklistItems { get; set; }
    public decimal Budget { get; set; }
    public decimal BudgetUtilized { get; set; }
    public string? Status { get; set; }  // "Upcoming", "InProgress", "Completed"
    public int UnreadMessageCount { get; set; }
}