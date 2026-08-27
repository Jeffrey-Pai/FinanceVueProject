namespace BankApi.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int FromAccountId { get; set; }
    public Account FromAccount { get; set; } = null!;
    public int? ToAccountId { get; set; }
    public Account? ToAccount { get; set; }
}
