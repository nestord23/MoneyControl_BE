using FluentValidation;
using MoneyControl.DTOs;

namespace MoneyControl.Validators;

public class CreateLoanValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, false);
        RuleFor(x => x.LenderName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Date).NotEmpty();
    }
}

public class UpdateLoanValidator : AbstractValidator<UpdateLoanRequest>
{
    public UpdateLoanValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, false);
        RuleFor(x => x.LenderName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
