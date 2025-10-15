using System.Globalization;
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

    public async Task<ExpenseResponseDTO> CreateAsync(ExpenseCreateDTO expenseCreateDto, int userId)
    {
        // Kreiramo entitet – datum je DateTime
        var newExpense = new Expense
        {
            Title = expenseCreateDto.Title.Trim(),
            Amount = expenseCreateDto.Amount,
            Category = expenseCreateDto.Category, // enum
            Date = expenseCreateDto.Date.Date, // samo datum, vrijeme 00:00:00
            Description = expenseCreateDto.Description?.Trim(),
            UserId = userId
        };

        await _dbContext.Expenses.AddAsync(newExpense);
        await _dbContext.SaveChangesAsync();

        // Mapiranje u response DTO – datum je string za frontend
        var response = new ExpenseResponseDTO
        {
            Id = newExpense.Id,
            Title = newExpense.Title,
            Amount = newExpense.Amount,
            Category = newExpense.Category.ToString(), // string
            Date = newExpense.Date.ToString("dd.MM.yyyy"), // string formatiran
            Description = newExpense.Description
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
            Category = e.Category.ToString(),
            Date = e.Date.ToString("dd.MM.yyyy"),
            Description = e.Description
        }).ToList();

        return expenseResponses;
    }

    public async Task<ExpenseResponseDTO> GetByIDAsync(int id, int userId)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
            throw new Exception($"Expense with id {id} not found");

        var expenseResponse = new ExpenseResponseDTO
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category.ToString(),
            Date = expense.Date.ToString("dd.MM.yyyy"),
            Description = expense.Description,
        };

        return expenseResponse;
    }

    public async Task<ExpenseResponseDTO> UpdateAsync(int id, int userId, decimal newAmount)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
            throw new Exception($"Expense with id {id} not found");

        if (newAmount != expense.Amount && newAmount <= 0)
            throw new Exception($"Expense with id {id} is invalid");

        expense.Amount = newAmount;

        await _dbContext.SaveChangesAsync();

        var expenseResponse = new ExpenseResponseDTO
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category.ToString(),
            Date = expense.Date.ToString("dd.MM.yyyy"),
            Description = expense.Description,
        };

        return expenseResponse;
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var expense = await _dbContext.Expenses.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
            throw new Exception($"Expense with id {id} not found");

        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ExpenseResponseDTO>> GetAllByConditionsAsync(int userId, ExpenseFilterDTO expenseFilter)
    {
        var expenses = _dbContext.Expenses.Where(e => e.UserId == userId);

        if (expenseFilter.Category.HasValue)
        {
            expenses = expenses.Where(e => e.Category == expenseFilter.Category.Value);
        }
        
        switch (expenseFilter?.Period)
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

            case TimeFilter.Custom:

                if (expenseFilter.StartDate.HasValue)
                    expenses = expenses.Where(e => e.Date >= expenseFilter.StartDate.Value.Date);

                if (expenseFilter.EndDate.HasValue)
                    expenses = expenses.Where(e => e.Date <= expenseFilter.EndDate.Value.Date);
                break;

            default:
                break;
        }
        
        if (!string.IsNullOrEmpty(expenseFilter.SortBy))
        {
            switch (expenseFilter.SortBy.ToLower())
            {
                case "date":
                    expenses = expenseFilter.OrderBy?.ToLower() == "desc" ?
                        expenses.OrderByDescending(e => e.Date) : 
                        expenses.OrderBy(e => e.Date);
                    break;
                
                case "amount":
                    expenses =  expenseFilter.OrderBy?.ToLower() == "desc" ?
                        expenses.OrderByDescending(e => e.Amount) :
                        expenses.OrderBy(e => e.Amount);
                    break;
                
                default:
                    expenses = expenses.OrderBy(e => e.Date);
                    break;
            }   
        }

        var response = await expenses.Select(e => new ExpenseResponseDTO
        {
            Id = e.Id,
            Title = e.Title,
            Amount = e.Amount,
            Category = e.Category.ToString(),
            Date = e.Date.ToString("dd.MM.yyyy"),
            Description = e.Description,
        }).ToListAsync();
        
        return response;
    }
}