namespace ExpenceTrackerAPI.Models.DTOs;

public class UserDTO
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}