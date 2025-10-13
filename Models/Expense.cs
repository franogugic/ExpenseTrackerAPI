using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenceTrackerAPI.Models;

public enum ExpenseCategory
{
    Groceries,
    Leisure,
    Electronics,
    Utilities,
    Clothing,
    Health,
    Others
}

public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }                 

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }             

    [Required]
    public ExpenseCategory Category { get; set; }   

    public string? Description { get; set; }        

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow; 

    public User User { get; set; } = null!;
}