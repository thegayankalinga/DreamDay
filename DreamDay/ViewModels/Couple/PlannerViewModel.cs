namespace DreamDay.ViewModels.Couple;

public class PlannerViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsApproved { get; set; }
    public bool IsRequested { get; set; }
}