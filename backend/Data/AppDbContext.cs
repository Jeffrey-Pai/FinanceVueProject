using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasIndex(a => a.AccountNumber).IsUnique();
            e.Property(a => a.Balance).HasColumnType("decimal(18,2)");
            e.HasOne(a => a.User).WithMany(u => u.Accounts).HasForeignKey(a => a.UserId);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.HasOne(t => t.FromAccount).WithMany(a => a.TransactionsFrom)
                .HasForeignKey(t => t.FromAccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ToAccount).WithMany(a => a.TransactionsTo)
                .HasForeignKey(t => t.ToAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
