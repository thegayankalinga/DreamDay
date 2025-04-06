using DreamDay.Models;

namespace DreamDay.ViewModels;

public class UserEditViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    // Couple-specific
    public string? PartnerName { get; set; }
    public DateOnly? WeddingDate { get; set; }
    public string? PlannerId { get; set; }

    // Planner-specific
    public bool IsApproved { get; set; }
}