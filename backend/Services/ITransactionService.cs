using BankApi.DTOs;

namespace BankApi.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetTransactionsAsync(int accountId, int userId, int page, int pageSize);
}
