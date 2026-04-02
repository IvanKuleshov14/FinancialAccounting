using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Targets;
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
            modelBuilder.ApplyConfiguration(new TargetConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
