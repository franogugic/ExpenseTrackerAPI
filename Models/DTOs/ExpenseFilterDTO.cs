using ExpenceTrackerAPI.Models.Enums;

namespace ExpenceTrackerAPI.Models.DTOs;

public class ExpenseFilterDTO
{
    public TimeFilter? Period { get; set; }      
    public DateTime? StartDate { get; set; }  
    public DateTime? EndDate { get; set; }  
    public ExpenseCategory? Category { get; set; }
    
    public string? SortBy { get; set; }
    
    public string? OrderBy { get; set; }
}
    