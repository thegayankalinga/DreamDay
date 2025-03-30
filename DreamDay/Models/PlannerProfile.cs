namespace DreamDay.Models;

public class PlannerProfile
{
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    public bool IsApproved { get; set; }
}