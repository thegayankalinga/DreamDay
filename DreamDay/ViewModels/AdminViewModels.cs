using DreamDay.Models;

namespace DreamDay.ViewModels;

public class DashboardUserViewModel
{
    public string Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public CoupleProfile? CoupleProfile { get; set; }
    public List<string> Roles { get; set; } = new();

    public string RoleString => Roles.Any() ? string.Join(", ", Roles) : "N/A";
}
public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalCouples { get; set; }
    public int TotalPlanners { get; set; }
    public int TotalVendors { get; set; }
    public int TotalVenues { get; set; }
    public List<DashboardUserViewModel> RecentUsers { get; set; } = new();
    public List<DashboardUserViewModel> UpcomingWeddings { get; set; } = new();
}

public class AdminReportViewModel
{
    public List<VenuePopularityViewModel> PopularVenues { get; set; } = new List<VenuePopularityViewModel>();
    public decimal AverageBudget { get; set; }
    public List<MonthlyRegistrationViewModel> MonthlyRegistrations { get; set; } = new List<MonthlyRegistrationViewModel>();
}

public class VenuePopularityViewModel
{
    public string VenueName { get; set; }
    public int BookingCount { get; set; }
}

public class MonthlyRegistrationViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
        
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
}