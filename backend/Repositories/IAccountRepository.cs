using BankApi.Models;

namespace BankApi.Repositories;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetByUserIdAsync(int userId);
    Task<Account?> GetByIdAsync(int id);
    Task<Account> CreateAsync(Account account);
    Task UpdateAsync(Account account);
}
