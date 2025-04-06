using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("BudgetCategory")]
    public int BudgetCategoryId { get; set; }
    public BudgetCategory? BudgetCategory { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;
    public DateTime ExpenseDate { get; set; }
    
}