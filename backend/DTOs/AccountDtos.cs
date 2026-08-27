namespace BankApi.DTOs;

public record AccountDto(int Id, string AccountNumber, string AccountType, decimal Balance, DateTime CreatedAt);
public record DepositWithdrawRequest(int AccountId, decimal Amount, string? Description);
public record TransferRequest(int FromAccountId, int ToAccountId, decimal Amount, string? Description);
