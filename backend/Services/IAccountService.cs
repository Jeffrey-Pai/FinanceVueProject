using BankApi.DTOs;

namespace BankApi.Services;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAccountsAsync(int userId);
    Task<AccountDto> DepositAsync(DepositWithdrawRequest request, int userId);
    Task<AccountDto> WithdrawAsync(DepositWithdrawRequest request, int userId);
    Task TransferAsync(TransferRequest request, int userId);
}
