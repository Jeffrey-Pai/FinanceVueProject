using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;
    public AccountRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Account>> GetByUserIdAsync(int userId) =>
        await _db.Accounts.Where(a => a.UserId == userId).ToListAsync();

    public Task<Account?> GetByIdAsync(int id) =>
        _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Account> CreateAsync(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAsync(Account account)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync();
    }
}
