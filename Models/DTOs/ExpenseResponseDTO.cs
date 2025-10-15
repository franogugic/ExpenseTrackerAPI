using ExpenceTrackerAPI.Models.Enums;

namespace ExpenceTrackerAPI.Models.DTOs;

public class ExpenseResponseDTO
{
        public int Id { get; set; }    
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public ExpenseCategory Category { get; set; }
        public string? Description { get; set; }
}