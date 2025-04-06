using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels
{
    public class PlannerDashboardViewModel
    {
        public int ActiveWeddingsCount { get; set; }
        public int PendingTasksCount { get; set; }
        public int UpcomingWeddingsCount { get; set; }
        public int UnreadMessagesCount { get; set; }
        public List<TaskViewModel> RecentTasks { get; set; } = new List<TaskViewModel>();
        public List<WeddingListItemViewModel> Weddings { get; set; } = new List<WeddingListItemViewModel>();
    }
    
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string WeddingName { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
    
    public class WeddingListItemViewModel
    {
        public int Id { get; set; }
        public string CoupleName { get; set; }
        public DateTime WeddingDate { get; set; }
        public string VenueName { get; set; }
        public int ChecklistProgress { get; set; }
        public string Status { get; set; }
    }
    
    public class WeddingDetailsViewModel
    {
        public int Id { get; set; }
        public string CoupleName { get; set; }
        public string CoupleEmail { get; set; }
        public string CouplePhone { get; set; }
        public DateTime WeddingDate { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public int GuestCount { get; set; }
        public decimal BudgetTotal { get; set; }
        public decimal BudgetSpent { get; set; }
        public int ChecklistProgress { get; set; }
        public string ImportantNotes { get; set; }
    }
    
    public class ManageChecklistViewModel
    {
        public int WeddingId { get; set; }
        public string CoupleName { get; set; }
        public DateTime WeddingDate { get; set; }
        public List<ChecklistItemViewModel> ChecklistItems { get; set; } = new List<ChecklistItemViewModel>();
    }
    
    public class ChecklistItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
    }
    
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
    }
    
    public class SendMessageViewModel
    {
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }
        public int WeddingId { get; set; }
        public string WeddingName { get; set; }
        
        [Required]
        public string Subject { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
    
    // Report View Models
    public class VenueReportViewModel
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string Address { get; set; }
        public int BookingCount { get; set; }
    }
    
    public class BudgetCategoryReportViewModel
    {
        public string Category { get; set; }
        public decimal AverageBudget { get; set; }
        public decimal AverageSpent { get; set; }
        public int WeddingCount { get; set; }
    }
    
    public class OverallBudgetViewModel
    {
        public decimal AverageTotalBudget { get; set; }
        public decimal AverageTotalSpent { get; set; }
        public decimal MaxBudget { get; set; }
        public decimal MinBudget { get; set; }
        public int TotalWeddingsCount { get; set; }
    }
    
    public class BudgetReportViewModel
    {
        public List<BudgetCategoryReportViewModel> CategoryReports { get; set; }
        public OverallBudgetViewModel OverallStats { get; set; }
    }
    
    public class VendorPerformanceViewModel
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Category { get; set; }
        public int BookingCount { get; set; }
        public double AverageRating { get; set; }
    }
}