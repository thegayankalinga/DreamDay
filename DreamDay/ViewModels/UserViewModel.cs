using DreamDay.Models;

namespace DreamDay.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();
    public string RoleString => Roles.Any() ? string.Join(", ", Roles) : "Unknown";
    public bool IsPlanner => Roles.Contains("Planner");
    public bool IsCouple => Roles.Contains("Couple");
    public bool IsAdmin => Roles.Contains("Admin");
    public bool? IsApproved { get; set; }
    public DateTime? CreatedAt { get; set; }
}