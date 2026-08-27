using BankApi.DTOs;
using BankApi.Repositories;

namespace BankApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly IAccountRepository _accountRepo;

    public TransactionService(ITransactionRepository transactionRepo, IAccountRepository accountRepo)
    {
        _transactionRepo = transactionRepo;
        _accountRepo = accountRepo;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(int accountId, int userId, int page, int pageSize)
    {
        var account = await _accountRepo.GetByIdAsync(accountId)
            ?? throw new KeyNotFoundException("Account not found.");
        if (account.UserId != userId) throw new UnauthorizedAccessException("Access denied.");

        var paged = await _transactionRepo.GetByAccountIdAsync(accountId, page, pageSize);
        var dtos = paged.Items.Select(t => new TransactionDto(
            t.Id, t.Type, t.Amount, t.Description, t.CreatedAt,
            t.FromAccountId, t.FromAccount.AccountNumber,
            t.ToAccountId, t.ToAccount?.AccountNumber));

        return new PagedResult<TransactionDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
