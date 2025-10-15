public class ExpenseResponseDTO
{
    public int Id { get; set; }    
    public string Title { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Date { get; set; }      
    public string Category { get; set; }  
    public string? Description { get; set; }
}