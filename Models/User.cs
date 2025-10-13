using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ExpenceTrackerAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public ICollection<Expense> Expenses { get; set; } = null!;
}