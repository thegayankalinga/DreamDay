using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels;

public class CreateItemViewModel
{
    [Required]
    public int ChecklistId { get; set; }
    [Required]
    public string ItemName { get; set; }
    public string? ItemDescription { get; set; }
    public DateTime? ItemDueDate { get; set; }
    public DateTime? ItemCheckDate { get; set; }
    public bool IsChecked { get; set; } = false;

}