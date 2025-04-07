using System.Collections;

namespace DreamDay.ViewModels.Couple;

public class AvailablePlannerViewModel
{
    public IEnumerable<PlannerViewModel> Planners { get; set; } = new List<PlannerViewModel>();
    public string RequestMessage { get; set; }
    
}