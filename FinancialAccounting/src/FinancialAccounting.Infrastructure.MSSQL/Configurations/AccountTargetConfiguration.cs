using FinancialAccounting.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class AccountTargetConfiguration : IEntityTypeConfiguration<AccountTarget>
    {
        public void Configure(EntityTypeBuilder<AccountTarget> builder)
        {
            builder.
                HasKey(t => t.Id);

            builder.
                HasOne(t => t.Account).
                WithOne(a => a.Target).
                HasForeignKey<AccountTarget>(t => t.AccountId).
                OnDelete(DeleteBehavior.Cascade);

            builder.
                Property(t => t.Name).
                HasMaxLength(30).
                IsRequired();

            builder.
                Property(t => t.Goal).
                HasPrecision(18, 2).
                IsRequired();
        }
    }
}
