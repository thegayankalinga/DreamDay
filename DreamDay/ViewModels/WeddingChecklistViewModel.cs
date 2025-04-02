using DreamDay.Models;

namespace DreamDay.ViewModels;

public class WeddingChecklistViewModel
{
    public int Id { get; set; }
    public int CoupleProfileId { get; set; }
    public CoupleProfile CoupleProfile { get; set; }
    public string CoupleFullName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public WeddingChecklistItem? NewItem { get; set; } // 👈 for the form
    public ICollection<WeddingChecklistItem>? Items { get; set; }
}