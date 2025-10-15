using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using ExpenceTrackerAPI.Models;
using ExpenceTrackerAPI.Models.DTOs;
using ExpenceTrackerAPI.Models.Enums;
using ExpenceTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

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

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ExpenseResponseDTO>>> GetAllAsync()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Unauthorized"));
        var expenses = await _expenseServices.GetAllAsync(userId);
        return Ok(expenses);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponseDTO>> GetAsync([FromRoute] int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Unauthorized"));
        var exponse = await _expenseServices.GetByIDAsync(id, userId);
        return Ok(exponse);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseResponseDTO>> UpdateAsync([FromRoute] int id, decimal Amount)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Unauthorized"));
        var updatedExpense = await _expenseServices.UpdateAsync(id, userId, Amount);
        return Ok(updatedExpense);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Unauthorized"));
        await _expenseServices.DeleteAsync(id, userId);
        return Ok();
    }

    [Authorize]
    [HttpGet("filter")]
    public async Task<ActionResult<List<Expense>>> GetByUserTimeAsync([FromQuery] TimeFilter filter)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Unauthorized"));
        var expenses = await _expenseServices.GetAllByTimeAsync(userId, filter);
        return Ok(expenses);
    }
}