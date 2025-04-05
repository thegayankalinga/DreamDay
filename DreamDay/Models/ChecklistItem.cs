using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DreamDay.Models;

public class ChecklistItem
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Checklist")]
    public int WeddingChecklistId { get; set; }
    [ValidateNever]
    public required Checklist Checklist { get; set; }
    
    [MaxLength(100)]
    public required string Title { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public bool IsCompleted { get; set; }
}