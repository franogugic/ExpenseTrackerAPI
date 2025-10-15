using ExpenceTrackerAPI.Models.DTOs;
using FluentValidation;

namespace ExpenceTrackerAPI.Validators;

public class ExpenseCreateDTOValidator : AbstractValidator<ExpenseCreateDTO>
{
    public ExpenseCreateDTOValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required!");
        
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero!");
        
        //RuleFor(x => x.Date)
            //.GreaterThan(DateTime.UtcNow).WithMessage("Date cannot be in the future!");
        
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category is required!");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}