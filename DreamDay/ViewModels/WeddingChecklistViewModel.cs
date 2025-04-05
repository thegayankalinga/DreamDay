using DreamDay.Models;

namespace DreamDay.ViewModels;

public class WeddingChecklistViewModel
{
    public int Id { get; set; }
    public int CoupleProfileId { get; set; }
    public required CoupleProfile CoupleProfile { get; set; }
    public required string CoupleFullName { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public ChecklistItem? NewItem { get; set; } // 👈 for the form
    public ICollection<ChecklistItem>? Items { get; set; }
}