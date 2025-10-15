using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenceTrackerAPI.Models.Enums;

namespace ExpenceTrackerAPI.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }          
    
    [Required]
    public string Title { get; set; } = null!;

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