using ExpenceTrackerAPI.Models.Enums;

public class ExpenseCreateDTO
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; set; } 
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}