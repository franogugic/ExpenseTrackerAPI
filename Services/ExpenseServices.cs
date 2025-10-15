using ExpenceTrackerAPI.Data;
using ExpenceTrackerAPI.Models;
using ExpenceTrackerAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ExpenceTrackerAPI.Models.Enums;

namespace ExpenceTrackerAPI.Services;

public class ExpenseServices
{
    private readonly AppDbContext _dbContext;

    public ExpenseServices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseResponseDTO> CreateAsync(ExpenseCreateDTO expensecreatedto, int userId)
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

        var response = new ExpenseResponseDTO
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

    public async Task<List<ExpenseResponseDTO>> GetAllAsync(int userId)
    {
        var expenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .ToListAsync(); 

        var expenseResponses = expenses.Select(e => new ExpenseResponseDTO
        {
            Id = e.Id,
            Title = e.Title,
            Amount = e.Amount,
            Category = e.Category, 
            Date = e.Date,
            Description = e.Description
        }).ToList();

        return expenseResponses;
    }

    public async Task<ExpenseResponseDTO> GetByIDAsync(int id, int userId)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if(expense == null) 
            throw new Exception($"Expense with id {id} not found");
        
        var expenseResponse = new ExpenseResponseDTO
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            Description = expense.Description,
        };
        
        return expenseResponse;
    }

    public async Task<ExpenseResponseDTO> UpdateAsync(int id, int userId, decimal newAmount)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if(expense == null)
            throw new Exception($"Expense with id {id} not found");
        
        if(newAmount != expense.Amount && newAmount <= 0)
            throw new Exception($"Expense with id {id} is invalid");
        
        expense.Amount = newAmount;
        
        await _dbContext.SaveChangesAsync();

        var expenseResponse = new ExpenseResponseDTO
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            Description = expense.Description,
        };
        
        return expenseResponse;
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if(expense == null)
            throw new Exception($"Expense with id {id} not found");
        
        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Expense>> GetAllByTimeAsync(int userId, TimeFilter period)
    {
        var expenses = _dbContext.Expenses.Where(e => e.UserId == userId);
        
        switch (period)
        {
            case TimeFilter.PastWeek:
                var time7Days = DateTime.UtcNow.AddDays(-7);
                expenses = expenses.Where(e => e.Date >= time7Days);
                break;

            case TimeFilter.LastMonth:
                var time1month = DateTime.UtcNow.AddMonths(-1);
                expenses = expenses.Where(e => e.Date >= time1month);
                break;

            case TimeFilter.Last3Months:
                var time3month = DateTime.UtcNow.AddMonths(-3);
                expenses = expenses.Where(e => e.Date >= time3month);
                break;
            
            case TimeFilter.Last6Months:
                var time6month = DateTime.UtcNow.AddMonths(-6);
                expenses = expenses.Where(e => e.Date >= time6month);
                break;
            
            case TimeFilter.LastYear:
                var time1year = DateTime.UtcNow.AddYears(-1);
                expenses = expenses.Where(e => e.Date >= time1year);
                break;

            /*case TimeFilter.Custom:
                expenses = expenses.Where(e => e.Date >= startDate.Date && e.Date <= endDate.Date);
                break;*/
            
            default:
                break;
        }
        
        return await expenses.ToListAsync();

    }
}


