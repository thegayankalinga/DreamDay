﻿using DreamDay.Data.Enums;
using DreamDay.Models;
using DreamDay.ViewModels.Couple;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DreamDay.ViewModels;

public class CoupleDashboardViewModel
{
    public required string FullCoupleName { get; set; }
    public DateOnly WeddingDate { get; set; }
    public List<Checklist> Checklists { get; set; } = new List<Checklist>();

    // Future features placeholders
    public bool IsGuestListReady => false;
    public List<Guest> Guests { get; set; } = new List<Guest>();
    public bool IsBudgetTrackerReady => false;
    public bool IsTimelineReady => false;
    
    public BudgetSummaryViewModel? BudgetSummary { get; set; }
    public IEnumerable<SelectListItem>? BudgetCategories { get; set; }
    public AddExpenseVeiwModel? NewExpense { get; set; }
    
    public List<WeddingEvent>? WeddingEvents { get; set; }
    
    public string? PlannerName { get; set; }
    public bool HasPlanner => !string.IsNullOrEmpty(PlannerName);
    
    public List<PlannerRequestViewModel> PlannerRequests { get; set; } = new List<PlannerRequestViewModel>();
    public string? AcceptedPlannerName { get; set; }
    public bool HasAcceptedPlanner => !string.IsNullOrEmpty(AcceptedPlannerName);
    
    public PlannerRequestStatus PlannerRequestStatus { get; set; }

}