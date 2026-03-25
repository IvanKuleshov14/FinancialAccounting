using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Targets;
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
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Target> Targets { get; set; }
    }
}
