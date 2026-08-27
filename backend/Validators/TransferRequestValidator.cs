using BankApi.DTOs;
using FluentValidation;

namespace BankApi.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.FromAccountId).GreaterThan(0);
        RuleFor(x => x.ToAccountId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(1000000);
        RuleFor(x => x).Must(x => x.FromAccountId != x.ToAccountId)
            .WithMessage("Cannot transfer to the same account.");
    }
}
