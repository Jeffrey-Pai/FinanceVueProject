using System;
using BankApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BankApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BankApi.Models.Account", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("AccountNumber")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("AccountType")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<decimal>("Balance")
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<int>("UserId")
                    .HasColumnType("int");

                b.HasKey("Id");
                b.HasIndex("AccountNumber").IsUnique();
                b.HasIndex("UserId");
                b.ToTable("Accounts");
            });

            modelBuilder.Entity("BankApi.Models.Transaction", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<decimal>("Amount")
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("FromAccountId")
                    .HasColumnType("int");

                b.Property<int?>("ToAccountId")
                    .HasColumnType("int");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");
                b.HasIndex("FromAccountId");
                b.HasIndex("ToAccountId");
                b.ToTable("Transactions");
            });

            modelBuilder.Entity("BankApi.Models.User", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.HasIndex("Username").IsUnique();
                b.ToTable("Users");
            });

            modelBuilder.Entity("BankApi.Models.Account", b =>
            {
                b.HasOne("BankApi.Models.User", "User")
                    .WithMany("Accounts")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("User");
            });

            modelBuilder.Entity("BankApi.Models.Transaction", b =>
            {
                b.HasOne("BankApi.Models.Account", "FromAccount")
                    .WithMany("TransactionsFrom")
                    .HasForeignKey("FromAccountId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("BankApi.Models.Account", "ToAccount")
                    .WithMany("TransactionsTo")
                    .HasForeignKey("ToAccountId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.Navigation("FromAccount");
                b.Navigation("ToAccount");
            });

            modelBuilder.Entity("BankApi.Models.User", b =>
            {
                b.Navigation("Accounts");
            });

            modelBuilder.Entity("BankApi.Models.Account", b =>
            {
                b.Navigation("TransactionsFrom");
                b.Navigation("TransactionsTo");
            });
#pragma warning restore 612, 618
        }
    }
}
