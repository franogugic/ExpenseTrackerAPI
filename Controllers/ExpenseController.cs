using System.Security.Claims;
using ExpenceTrackerAPI.Models;
using ExpenceTrackerAPI.Models.DTOs;
using ExpenceTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTrackerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpenseController  : ControllerBase
{
    private readonly ExpenseServices _expenseServices;

    public ExpenseController(ExpenseServices expenseServices)
    {
        _expenseServices = expenseServices;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody]ExpenseCreateDTO  expenseCreateDTO)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not found")); 
        
        var respose = await _expenseServices.CreateAsync(expenseCreateDTO, userId);
        return Ok(respose);
    }
    
        
}