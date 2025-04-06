using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class BudgetCategory
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public required decimal AllocatedAmount { get; set; } = 0;
    
    [ForeignKey("AppUser")]
    [MaxLength(450)]
    public required string UserId { get; set; }
    public AppUser? AppUser { get; set; }
    public ICollection<Expense>? Expenses { get; set; }
}