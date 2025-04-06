// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using DreamDay.Data;
// using DreamDay.Models;
// using DreamDay.Services;
// using DreamDay.ViewModels;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace DreamDay.Controllers
// {
//     [Authorize(Roles = UserRoles.Planner)]
//     public class PlannerController : Controller
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly IWeddingService _weddingService;
//         private readonly IMessageService _messageService;
//         
//         public PlannerController(
//             ApplicationDbContext context,
//             UserManager<ApplicationUser> userManager,
//             IWeddingService weddingService,
//             IMessageService messageService)
//         {
//             _context = context;
//             _userManager = userManager;
//             _weddingService = weddingService;
//             _messageService = messageService;
//         }
//         
//         // GET: Planner Dashboard
//         public async Task<IActionResult> Index()
//         {
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             if (currentUser == null)
//                 return Challenge();
//                 
//             var viewModel = new PlannerViewModels
//             {
//                 ActiveWeddingsCount = await _context.Weddings
//                     .Where(w => w.PlannerId == currentUser.Id && w.WeddingDate > DateTime.Now)
//                     .CountAsync(),
//                     
//                 PendingTasksCount = await _context.ChecklistItems
//                     .Where(c => c.Wedding.PlannerId == currentUser.Id && !c.IsCompleted)
//                     .CountAsync(),
//                     
//                 UpcomingWeddingsCount = await _context.Weddings
//                     .Where(w => w.PlannerId == currentUser.Id && w.WeddingDate > DateTime.Now && w.WeddingDate <= DateTime.Now.AddMonths(1))
//                     .CountAsync(),
//                     
//                 UnreadMessagesCount = await _context.Messages
//                     .Where(m => m.RecipientId == currentUser.Id && !m.IsRead)
//                     .CountAsync(),
//                     
//                 RecentTasks = await _context.ChecklistItems
//                     .Where(c => c.Wedding.PlannerId == currentUser.Id)
//                     .OrderByDescending(c => c.DueDate)
//                     .Take(5)
//                     .Select(c => new TaskViewModel
//                     {
//                         Id = c.Id,
//                         TaskName = c.Name,
//                         WeddingName = $"{c.Wedding.Couple.FirstName} & {c.Wedding.Couple.PartnerFirstName}",
//                         DueDate = c.DueDate,
//                         Status = c.IsCompleted ? "Completed" : 
//                                 (c.DueDate < DateTime.Now ? "Overdue" : "Pending")
//                     })
//                     .ToListAsync(),
//                     
//                 Weddings = await _context.Weddings
//                     .Where(w => w.PlannerId == currentUser.Id)
//                     .OrderBy(w => w.WeddingDate)
//                     .Select(w => new WeddingListItemViewModel
//                     {
//                         Id = w.Id,
//                         CoupleName = $"{w.Couple.FirstName} & {w.Couple.PartnerFirstName}",
//                         WeddingDate = w.WeddingDate,
//                         VenueName = w.Venue != null ? w.Venue.Name : "Not Selected",
//                         ChecklistProgress = CalculateChecklistProgress(w.ChecklistItems),
//                         Status = w.WeddingDate > DateTime.Now ? "Active" : "Completed"
//                     })
//                     .ToListAsync()
//             };
//             
//             return View(viewModel);
//         }
//         
//         // GET: Active Weddings
//         public async Task<IActionResult> ActiveWeddings()
//         {
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             var activeWeddings = await _context.Weddings
//                 .Where(w => w.PlannerId == currentUser.Id && w.WeddingDate > DateTime.Now)
//                 .OrderBy(w => w.WeddingDate)
//                 .Select(w => new WeddingListItemViewModel
//                 {
//                     Id = w.Id,
//                     CoupleName = $"{w.Couple.FirstName} & {w.Couple.PartnerFirstName}",
//                     WeddingDate = w.WeddingDate,
//                     VenueName = w.Venue != null ? w.Venue.Name : "Not Selected",
//                     ChecklistProgress = CalculateChecklistProgress(w.ChecklistItems),
//                     Status = "Active"
//                 })
//                 .ToListAsync();
//                 
//             return View(activeWeddings);
//         }
//         
//         // GET: Wedding Details
//         public async Task<IActionResult> WeddingDetails(int id)
//         {
//             var wedding = await _context.Weddings
//                 .Include(w => w.Couple)
//                 .Include(w => w.Venue)
//                 .Include(w => w.ChecklistItems)
//                 .Include(w => w.Guests)
//                 .Include(w => w.Budget)
//                 .FirstOrDefaultAsync(w => w.Id == id);
//                 
//             if (wedding == null)
//                 return NotFound();
//                 
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             if (wedding.PlannerId != currentUser.Id)
//                 return Forbid();
//                 
//             var viewModel = new WeddingDetailsViewModel
//             {
//                 Id = wedding.Id,
//                 CoupleName = $"{wedding.Couple.FirstName} & {wedding.Couple.PartnerFirstName}",
//                 CoupleEmail = wedding.Couple.Email,
//                 CouplePhone = wedding.Couple.PhoneNumber,
//                 WeddingDate = wedding.WeddingDate,
//                 VenueName = wedding.Venue?.Name ?? "Not Selected",
//                 VenueAddress = wedding.Venue?.Address ?? "",
//                 GuestCount = wedding.Guests.Count,
//                 BudgetTotal = wedding.Budget?.TotalBudget ?? 0,
//                 BudgetSpent = wedding.Budget?.TotalSpent ?? 0,
//                 ChecklistProgress = CalculateChecklistProgress(wedding.ChecklistItems),
//                 ImportantNotes = wedding.Notes
//             };
//             
//             return View(viewModel);
//         }
//         
//         // GET: Manage Checklist
//         
//         // POST: Add Checklist Item
// [HttpPost]
// public async Task<IActionResult> AddChecklistItem(ChecklistItemViewModel model)
// {
//     if (!ModelState.IsValid)
//     {
//         TempData["ErrorMessage"] = "Invalid form data. Please check your inputs.";
//         return RedirectToAction(nameof(ManageChecklist), new { id = model.WeddingId });
//     }
//     
//     var wedding = await _context.Weddings
//         .FirstOrDefaultAsync(w => w.Id == model.WeddingId);
//         
//     if (wedding == null)
//         return NotFound();
//         
//     var currentUser = await _userManager.GetUserAsync(User);
//     
//     if (wedding.PlannerId != currentUser.Id)
//         return Forbid();
//         
//     var checklistItem = new ChecklistItem
//     {
//         Name = model.Name,
//         Description = model.Description,
//         Category = model.Category,
//         DueDate = model.DueDate,
//         IsCompleted = false,
//         CompletedDate = null,
//         WeddingId = model.WeddingId,
//         CreatedDate = DateTime.Now,
//         CreatedById = currentUser.Id
//     };
//     
//     _context.ChecklistItems.Add(checklistItem);
//     await _context.SaveChangesAsync();
//     
//     TempData["SuccessMessage"] = "Task added successfully.";
//     
//     return RedirectToAction(nameof(ManageChecklist), new { id = model.WeddingId });
// }
//
// // POST: Edit Checklist Item
// [HttpPost]
// public async Task<IActionResult> EditChecklistItem(ChecklistItemViewModel model)
// {
//     if (!ModelState.IsValid)
//     {
//         TempData["ErrorMessage"] = "Invalid form data. Please check your inputs.";
//         return RedirectToAction(nameof(ManageChecklist), new { id = model.WeddingId });
//     }
//     
//     var checklistItem = await _context.ChecklistItems
//         .Include(c => c.Wedding)
//         .FirstOrDefaultAsync(c => c.Id == model.Id);
//         
//     if (checklistItem == null)
//         return NotFound();
//         
//     var currentUser = await _userManager.GetUserAsync(User);
//     
//     if (checklistItem.Wedding.PlannerId != currentUser.Id)
//         return Forbid();
//         
//     checklistItem.Name = model.Name;
//     checklistItem.Description = model.Description;
//     checklistItem.Category = model.Category;
//     checklistItem.DueDate = model.DueDate;
//     
//     // Update completion status
//     if (model.IsCompleted && !checklistItem.IsCompleted)
//     {
//         // Task is being marked as completed
//         checklistItem.IsCompleted = true;
//         checklistItem.CompletedDate = DateTime.Now;
//     }
//     else if (!model.IsCompleted && checklistItem.IsCompleted)
//     {
//         // Task is being unmarked
//         checklistItem.IsCompleted = false;
//         checklistItem.CompletedDate = null;
//     }
//     
//     checklistItem.LastUpdatedDate = DateTime.Now;
//     checklistItem.LastUpdatedById = currentUser.Id;
//     
//     await _context.SaveChangesAsync();
//     
//     TempData["SuccessMessage"] = "Task updated successfully.";
//     
//     return RedirectToAction(nameof(ManageChecklist), new { id = checklistItem.WeddingId });
// }
//
// // POST: Delete Checklist Item
// [HttpPost]
// public async Task<JsonResult> DeleteChecklistItem(int id)
// {
//     var checklistItem = await _context.ChecklistItems
//         .Include(c => c.Wedding)
//         .FirstOrDefaultAsync(c => c.Id == id);
//         
//     if (checklistItem == null)
//         return Json(new { success = false });
//         
//     var currentUser = await _userManager.GetUserAsync(User);
//     
//     if (checklistItem.Wedding.PlannerId != currentUser.Id)
//         return Json(new { success = false });
//         
//     _context.ChecklistItems.Remove(checklistItem);
//     await _context.SaveChangesAsync();
//     
//     return Json(new { success = true });
// }
//
// // POST: Update Checklist Item (AJAX)
// [HttpPost]
// public async Task<JsonResult> UpdateChecklistItem(int id, bool isCompleted)
// {
//     var checklistItem = await _context.ChecklistItems
//         .Include(c => c.Wedding)
//         .FirstOrDefaultAsync(c => c.Id == id);
//         
//     if (checklistItem == null)
//         return Json(new { success = false });
//         
//     var currentUser = await _userManager.GetUserAsync(User);
//     
//     if (checklistItem.Wedding.PlannerId != currentUser.Id)
//         return Json(new { success = false });
//         
//     checklistItem.IsCompleted = isCompleted;
//     
//     if (isCompleted)
//         checklistItem.CompletedDate = DateTime.Now;
//     else
//         checklistItem.CompletedDate = null;
//         
//     checklistItem.LastUpdatedDate = DateTime.Now;
//     checklistItem.LastUpdatedById = currentUser.Id;
//     
//     await _context.SaveChangesAsync();
//     
//     return Json(new { success = true });
// }
//
// // GET: Export Checklist
// public async Task<IActionResult> ExportChecklist(int id)
// {
//     var wedding = await _context.Weddings
//         .Include(w => w.Couple)
//         .Include(w => w.ChecklistItems)
//         .FirstOrDefaultAsync(w => w.Id == id);
//         
//     if (wedding == null)
//         return NotFound();
//         
//     var currentUser = await _userManager.GetUserAsync(User);
//     
//     if (wedding.PlannerId != currentUser.Id)
//         return Forbid();
//         
//     // Generate a CSV format for export
//     var csvContent = new StringBuilder();
//     csvContent.AppendLine("Task,Category,Description,Due Date,Status");
//     
//     foreach (var item in wedding.ChecklistItems.OrderBy(c => c.DueDate))
//     {
//         string status = item.IsCompleted ? "Completed" : 
//                         (item.DueDate < DateTime.Now ? "Overdue" : "Pending");
//                         
//         csvContent.AppendLine($"\"{item.Name}\",\"{item.Category}\",\"{item.Description}\",\"{item.DueDate:MM/dd/yyyy}\",\"{status}\"");
//     }
//     
//     var fileName = $"{wedding.Couple.FirstName}_{wedding.Couple.PartnerFirstName}_Checklist_{DateTime.Now:yyyyMMdd}.csv";
//     
//     return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", fileName);
// }
//         public async Task<IActionResult> ManageChecklist(int id)
//         {
//             var wedding = await _context.Weddings
//                 .Include(w => w.Couple)
//                 .Include(w => w.ChecklistItems)
//                 .FirstOrDefaultAsync(w => w.Id == id);
//                 
//             if (wedding == null)
//                 return NotFound();
//                 
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             if (wedding.PlannerId != currentUser.Id)
//                 return Forbid();
//                 
//             var viewModel = new ManageChecklistViewModel
//             {
//                 WeddingId = wedding.Id,
//                 CoupleName = $"{wedding.Couple.FirstName} & {wedding.Couple.PartnerFirstName}",
//                 WeddingDate = wedding.WeddingDate,
//                 
//                 ChecklistItems = wedding.ChecklistItems
//                     .OrderBy(c => c.DueDate)
//                     .Select(c => new ChecklistItemViewModel
//                     {
//                         Id = c.Id,
//                         Name = c.Name,
//                         Description = c.Description,
//                         Category = c.Category,
//                         DueDate = c.DueDate,
//                         IsCompleted = c.IsCompleted,
//                         CompletedDate = c.CompletedDate,
//                         Status = c.IsCompleted ? "Completed" : 
//                                 (c.DueDate < DateTime.Now ? "Overdue" : "Pending")
//                     })
//                     .ToList()
//             };
//             
//             return View(viewModel);
//         }
//         
//         // POST: Update Checklist Item
//         [HttpPost]
//         public async Task<IActionResult> UpdateChecklistItem(int id, bool isCompleted)
//         {
//             var checklistItem = await _context.ChecklistItems
//                 .Include(c => c.Wedding)
//                 .FirstOrDefaultAsync(c => c.Id == id);
//                 
//             if (checklistItem == null)
//                 return NotFound();
//                 
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             if (checklistItem.Wedding.PlannerId != currentUser.Id)
//                 return Forbid();
//                 
//             checklistItem.IsCompleted = isCompleted;
//             
//             if (isCompleted)
//                 checklistItem.CompletedDate = DateTime.Now;
//             else
//                 checklistItem.CompletedDate = null;
//                 
//             await _context.SaveChangesAsync();
//             
//             return Json(new { success = true });
//         }
//         
//         // GET: Messages
//         public async Task<IActionResult> Messages()
//         {
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             var messages = await _context.Messages
//                 .Where(m => m.RecipientId == currentUser.Id)
//                 .OrderByDescending(m => m.SentDate)
//                 .Select(m => new MessageViewModel
//                 {
//                     Id = m.Id,
//                     SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
//                     Subject = m.Subject,
//                     Content = m.Content,
//                     SentDate = m.SentDate,
//                     IsRead = m.IsRead
//                 })
//                 .ToListAsync();
//                 
//             return View(messages);
//         }
//         
//         // GET: Message Couple
//         public async Task<IActionResult> MessageCouple(int id)
//         {
//             var wedding = await _context.Weddings
//                 .Include(w => w.Couple)
//                 .FirstOrDefaultAsync(w => w.Id == id);
//                 
//             if (wedding == null)
//                 return NotFound();
//                 
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             if (wedding.PlannerId != currentUser.Id)
//                 return Forbid();
//                 
//             var viewModel = new SendMessageViewModel
//             {
//                 RecipientId = wedding.CoupleId,
//                 RecipientName = $"{wedding.Couple.FirstName} {wedding.Couple.LastName}",
//                 WeddingId = wedding.Id,
//                 WeddingName = $"{wedding.Couple.FirstName} & {wedding.Couple.PartnerFirstName}'s Wedding"
//             };
//             
//             return View(viewModel);
//         }
//         
//         // POST: Send Message
//         [HttpPost]
//         public async Task<IActionResult> SendMessage(SendMessageViewModel model)
//         {
//             if (!ModelState.IsValid)
//                 return View(model);
//                 
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             var message = new Message
//             {
//                 SenderId = currentUser.Id,
//                 RecipientId = model.RecipientId,
//                 Subject = model.Subject,
//                 Content = model.Content,
//                 SentDate = DateTime.Now,
//                 IsRead = false,
//                 WeddingId = model.WeddingId
//             };
//             
//             _context.Messages.Add(message);
//             await _context.SaveChangesAsync();
//             
//             TempData["SuccessMessage"] = "Message sent successfully.";
//             
//             return RedirectToAction(nameof(WeddingDetails), new { id = model.WeddingId });
//         }
//         
//         // GET: Calendar Events (AJAX)
//         [HttpGet]
//         public async Task<JsonResult> GetCalendarEvents()
//         {
//             var currentUser = await _userManager.GetUserAsync(User);
//             
//             var weddings = await _context.Weddings
//                 .Where(w => w.PlannerId == currentUser.Id)
//                 .Select(w => new
//                 {
//                     id = "wedding_" + w.Id,
//                     title = $"{w.Couple.FirstName} & {w.Couple.PartnerFirstName}'s Wedding",
//                     start = w.WeddingDate.ToString("yyyy-MM-dd"),
//                     backgroundColor = "#4e73df",
//                     borderColor = "#4e73df"
//                 })
//                 .ToListAsync();
//                 
//             var tasks = await _context.ChecklistItems
//                 .Where(c => c.Wedding.PlannerId == currentUser.Id && !c.IsCompleted)
//                 .Select(c => new
//                 {
//                     id = "task_" + c.Id,
//                     title = c.Name + " - " + c.Wedding.Couple.FirstName + " & " + c.Wedding.Couple.PartnerFirstName,
//                     start = c.DueDate.ToString("yyyy-MM-dd"),
//                     backgroundColor = c.DueDate < DateTime.Now ? "#e74a3b" : "#1cc88a",
//                     borderColor = c.DueDate < DateTime.Now ? "#e74a3b" : "#1cc88a"
//                 })
//                 .ToListAsync();
//                 
//             var events = weddings.Concat(tasks).ToList();
//             
//             return Json(events);
//         }
//         
//         // GET: Reports Dashboard
//         public IActionResult Reports()
//         {
//             return View();
//         }
//         
//         // GET: Popular Venues Report
//         public async Task<IActionResult> PopularVenuesReport()
//         {
//             var popularVenues = await _context.Weddings
//                 .Where(w => w.VenueId != null)
//                 .GroupBy(w => w.VenueId)
//                 .Select(g => new
//                 {
//                     VenueId = g.Key,
//                     Count = g.Count()
//                 })
//                 .OrderByDescending(v => v.Count)
//                 .Take(10)
//                 .Join(_context.Venues,
//                     v => v.VenueId,
//                     venue => venue.Id,
//                     (v, venue) => new VenueReportViewModel
//                     {
//                         VenueId = venue.Id,
//                         VenueName = venue.Name,
//                         Address = venue.Address,
//                         BookingCount = v.Count
//                     })
//                 .ToListAsync();
//                 
//             return View(popularVenues);
//         }
//         
//         // GET: Average Budget Report
//         public async Task<IActionResult> AverageBudgetReport()
//         {
//             // Get average budget by category
//             var budgetCategories = await _context.BudgetItems
//                 .GroupBy(b => b.Category)
//                 .Select(g => new BudgetCategoryReportViewModel
//                 {
//                     Category = g.Key,
//                     AverageBudget = g.Average(b => b.PlannedAmount),
//                     AverageSpent = g.Average(b => b.ActualAmount),
//                     WeddingCount = g.Select(b => b.BudgetId).Distinct().Count()
//                 })
//                 .OrderByDescending(b => b.AverageBudget)
//                 .ToListAsync();
//                 
//             // Get overall budget statistics
//             var overallStats = new OverallBudgetViewModel
//             {
//                 AverageTotalBudget = await _context.Budgets.AverageAsync(b => b.TotalBudget),
//                 AverageTotalSpent = await _context.Budgets.AverageAsync(b => b.TotalSpent),
//                 MaxBudget = await _context.Budgets.MaxAsync(b => b.TotalBudget),
//                 MinBudget = await _context.Budgets.MinAsync(b => b.TotalBudget),
//                 TotalWeddingsCount = await _context.Budgets.CountAsync()
//             };
//             
//             var viewModel = new BudgetReportViewModel
//             {
//                 CategoryReports = budgetCategories,
//                 OverallStats = overallStats
//             };
//             
//             return View(viewModel);
//         }
//         
//         // GET: Vendor Performance Report
//         public async Task<IActionResult> VendorPerformanceReport()
//         {
//             var vendorPerformance = await _context.VendorBookings
//                 .GroupBy(v => v.VendorId)
//                 .Select(g => new
//                 {
//                     VendorId = g.Key,
//                     BookingCount = g.Count(),
//                     AverageRating = g.Average(v => v.Rating ?? 0)
//                 })
//                 .OrderByDescending(v => v.AverageRating)
//                 .ThenByDescending(v => v.BookingCount)
//                 .Take(10)
//                 .Join(_context.Vendors,
//                     v => v.VendorId,
//                     vendor => vendor.Id,
//                     (v, vendor) => new VendorPerformanceViewModel
//                     {
//                         VendorId = vendor.Id,
//                         VendorName = vendor.Name,
//                         Category = vendor.Category,
//                         BookingCount = v.BookingCount,
//                         AverageRating = v.AverageRating
//                     })
//                 .ToListAsync();
//                 
//             return View(vendorPerformance);
//         }
//         
//         // Helper method to calculate checklist progress percentage
//         private static int CalculateChecklistProgress(ICollection<ChecklistItem> items)
//         {
//             if (items == null || !items.Any())
//                 return 0;
//                 
//             int completedCount = items.Count(i => i.IsCompleted);
//             return (int)Math.Round((double)completedCount / items.Count * 100);
//         }
//     }
// }
