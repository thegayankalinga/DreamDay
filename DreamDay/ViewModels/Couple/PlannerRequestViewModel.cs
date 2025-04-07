using DreamDay.Data.Enums;

namespace DreamDay.ViewModels.Couple;

public class PlannerRequestViewModel
{
    public int RequestId { get; set; }
    public string PlannerId { get; set; }
    public string PlannerName { get; set; }
    public DateTime RequestDate { get; set; }
    public PlannerRequestStatus Status { get; set; }
}