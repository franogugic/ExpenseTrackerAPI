using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using ExpenceTrackerAPI.Data;
using ExpenceTrackerAPI.Models;
using ExpenceTrackerAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ExpenceTrackerAPI.Services;

public class UserServices 
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    
    public UserServices(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<UserResponseDTO> CreateAsync(UserDTO userdto)
    {
        if (string.IsNullOrWhiteSpace(userdto.Password) || string.IsNullOrWhiteSpace(userdto.FirstName) ||
            string.IsNullOrWhiteSpace(userdto.Email) || string.IsNullOrWhiteSpace(userdto.LastName))
        {
            throw new ArgumentException("First name, Last name, Email and Password must be provided");
        }
        
        try
        {
            var mail = new System.Net.Mail.MailAddress(userdto.Email);
            userdto.Email = mail.Address.Trim().ToLower();
        }   
        catch (Exception e)
        {
           throw new ArgumentException("Invalid email address provided", e); 
        }
        
        if(userdto.Password.Length < 6) throw new Exception("Password must be at least 6 characters long");
        if(!userdto.Password.Any(char.IsUpper)  || !userdto.Password.Any(char.IsDigit)) throw new Exception("Password must contain at least one uppercase letter and one Digit");
        if(userdto.Password.Contains(userdto.FirstName) || userdto.Password.Contains(userdto.LastName)) throw new Exception("Password cannot contain your name");
        
        string HashedPassword = BCrypt.Net.BCrypt.HashPassword(userdto.Password);
        
        if (await _dbContext.Users.AnyAsync(u => u.Email == userdto.Email))
            throw new Exception("User with this email already exists.");
        
        var newUser = new User
        {
            FirstName = userdto.FirstName,
            LastName = userdto.LastName,
            Email = userdto.Email,
            PasswordHash = HashedPassword,
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        var response = new UserResponseDTO
        {
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
        };
        
        return response;
    }

    public async Task<UserLoginResponseDTO> LoginAsync(UserLoginDTO userlogindto)
    {
        if (string.IsNullOrWhiteSpace(userlogindto.Password) || string.IsNullOrWhiteSpace(userlogindto.Email))
        {
            throw new ArgumentException("Password and Email must be provided");
        }
        
        User user = await _dbContext.Users.FirstOrDefaultAsync(doc => doc.Email == userlogindto.Email);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }
        
        bool authenticated = BCrypt.Net.BCrypt.Verify(userlogindto.Password, user.PasswordHash);
        if (!authenticated)
        {
            throw new UnauthorizedAccessException("Wrong password");
        }
        
        var token = GenerateJwtToken(user);
        var response = new UserLoginResponseDTO
        {
            Token = token,
            Email = userlogindto.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
        
        return response;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireHours),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}