using ExpenceTrackerAPI.Data;
using ExpenceTrackerAPI.Models;
using ExpenceTrackerAPI.Models.DTOs;
using ExpenceTrackerAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace ExpenceTrackerAPI.Services;

public class ExpenseServices
{
    private readonly AppDbContext _dbContext;

    public ExpenseServices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseCreateResponseDTO> CreateAsync(ExpenseCreateDTO expensecreatedto, int userId)
    {
        var newExpense = new Expense
        {
            Title = expensecreatedto.Title.Trim(),
            Amount = expensecreatedto.Amount,
            Category = expensecreatedto.Category,
            Date = expensecreatedto.Date.ToUniversalTime(),
            Description = expensecreatedto.Description?.Trim(),
            UserId = userId
        };
        
        await _dbContext.Expenses.AddAsync(newExpense);
        await _dbContext.SaveChangesAsync();

        var response = new ExpenseCreateResponseDTO
        {
            Id = newExpense.Id,
            Amount = newExpense.Amount,
            Category = newExpense.Category,
            Date = newExpense.Date,
            Description = newExpense.Description,
            Title = newExpense.Title,
        };
        return response;
    }
}