using BankApi.DTOs;
using FluentValidation;

namespace BankApi.Validators;

public class DepositWithdrawRequestValidator : AbstractValidator<DepositWithdrawRequest>
{
    public DepositWithdrawRequestValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(1000000);
    }
}
