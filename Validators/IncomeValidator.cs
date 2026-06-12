using FluentValidation;
using MoneyControl.DTOs;

namespace MoneyControl.Validators;

public class CreateIncomeValidator : AbstractValidator<CreateIncomeRequest>
{
    public CreateIncomeValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, false);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateIncomeValidator : AbstractValidator<UpdateIncomeRequest>
{
    public UpdateIncomeValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, false);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
