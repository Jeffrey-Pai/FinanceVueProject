using BankApi.Data;
using BankApi.DTOs;
using BankApi.Models;
using BankApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly AppDbContext _db;

    public AccountService(IAccountRepository accountRepo, ITransactionRepository transactionRepo, AppDbContext db)
    {
        _accountRepo = accountRepo;
        _transactionRepo = transactionRepo;
        _db = db;
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsAsync(int userId)
    {
        var accounts = await _accountRepo.GetByUserIdAsync(userId);
        return accounts.Select(a => new AccountDto(a.Id, a.AccountNumber, a.AccountType, a.Balance, a.CreatedAt));
    }

    public async Task<AccountDto> DepositAsync(DepositWithdrawRequest request, int userId)
    {
        var account = await _accountRepo.GetByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException("Account not found.");
        if (account.UserId != userId) throw new UnauthorizedAccessException("Access denied.");

        account.Balance += request.Amount;
        await _accountRepo.UpdateAsync(account);
        await _transactionRepo.CreateAsync(new Transaction
        {
            Type = "Deposit",
            Amount = request.Amount,
            Description = request.Description,
            FromAccountId = account.Id
        });
        return new AccountDto(account.Id, account.AccountNumber, account.AccountType, account.Balance, account.CreatedAt);
    }

    public async Task<AccountDto> WithdrawAsync(DepositWithdrawRequest request, int userId)
    {
        var account = await _accountRepo.GetByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException("Account not found.");
        if (account.UserId != userId) throw new UnauthorizedAccessException("Access denied.");
        if (account.Balance < request.Amount) throw new InvalidOperationException("Insufficient funds.");

        account.Balance -= request.Amount;
        await _accountRepo.UpdateAsync(account);
        await _transactionRepo.CreateAsync(new Transaction
        {
            Type = "Withdrawal",
            Amount = request.Amount,
            Description = request.Description,
            FromAccountId = account.Id
        });
        return new AccountDto(account.Id, account.AccountNumber, account.AccountType, account.Balance, account.CreatedAt);
    }

    public async Task TransferAsync(TransferRequest request, int userId)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var from = await _accountRepo.GetByIdAsync(request.FromAccountId)
                ?? throw new KeyNotFoundException("Source account not found.");
            if (from.UserId != userId) throw new UnauthorizedAccessException("Access denied.");
            if (from.Balance < request.Amount) throw new InvalidOperationException("Insufficient funds.");

            var to = await _accountRepo.GetByIdAsync(request.ToAccountId)
                ?? throw new KeyNotFoundException("Destination account not found.");

            from.Balance -= request.Amount;
            to.Balance += request.Amount;

            await _accountRepo.UpdateAsync(from);
            await _accountRepo.UpdateAsync(to);

            await _transactionRepo.CreateAsync(new Transaction
            {
                Type = "Transfer",
                Amount = request.Amount,
                Description = request.Description,
                FromAccountId = from.Id,
                ToAccountId = to.Id
            });

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
