using ExpenceTrackerAPI.Models.DTOs;
using ExpenceTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserServices _userServices;

    public UserController(UserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync([FromBody] UserDTO userdto)
    {
        try
        {
            var createdUser = await _userServices.CreateAsync(userdto);
            return CreatedAtAction(nameof(CreateAsync), createdUser);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDTO userlogindto)
    {
            var response = await _userServices.LoginAsync(userlogindto);
            return Ok(response);
    }
}