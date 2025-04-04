using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IUserProfileRepository
{
    Task InitializeAsync();
    string? UserType { get; }
    CoupleProfile? CoupleProfile { get; }
    PlannerProfile? PlannerProfile { get; }
    AppUser? CurrentUser { get; }
}