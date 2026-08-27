using BankApi.Data;
using BankApi.DTOs;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;
    public TransactionRepository(AppDbContext db) => _db = db;

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return transaction;
    }

    public async Task<PagedResult<Transaction>> GetByAccountIdAsync(int accountId, int page, int pageSize)
    {
        var query = _db.Transactions
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Transaction>(items, total, page, pageSize);
    }
}
