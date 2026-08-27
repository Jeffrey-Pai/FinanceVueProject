using BankApi.DTOs;
using BankApi.Models;

namespace BankApi.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<PagedResult<Transaction>> GetByAccountIdAsync(int accountId, int page, int pageSize);
}
