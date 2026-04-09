using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Categories;
using FinancialAccounting.Entities.Targets;
using FinancialAccounting.Entities.TargetTransactions;
using FinancialAccounting.Entities.Transactions;
using FinancialAccounting.Infrastructure.MSSQL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounting.Infrastructure.MSSQL.Data
{
    public class FinancialAccountingDbContext : DbContext
    {
        public FinancialAccountingDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTargetConfiguration());
            modelBuilder.ApplyConfiguration(new TargetConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new TargetTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTarget> AccountTargets { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TargetTransaction> TargetTransactions { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
