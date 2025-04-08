namespace DreamDay.ViewModels.Planner;

public class PlannerDashboardViewModel
{
    public required string PlannerName { get; set; }
    public int TotalWeddingsCount { get; set; }
    public int UpcomingWeddingsCount { get; set; }
    public int PastWeddingsCount { get; set; }
        
    public List<PlannerWeddingCardViewModel> AssignedWeddings { get; set; } = new();
    public List<RecentActivityViewModel> RecentActivities { get; set; } = new(); 
}


// Views/
//     └── Planner/
//     ├── Dashboard.cshtml       (Main dashboard view)
//     ├── WeddingDetails.cshtml  (Future: Detailed view of a specific wedding)
//     ├── ManageChecklists.cshtml (Future: Checklist management interface)
//     └── _WeddingCard.cshtml    (Partial view for individual wedding cards)