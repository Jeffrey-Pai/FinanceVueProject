namespace BankApi.DTOs;

public record TransactionDto(int Id, string Type, decimal Amount, string? Description, DateTime CreatedAt,
    int FromAccountId, string FromAccountNumber, int? ToAccountId, string? ToAccountNumber);

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
