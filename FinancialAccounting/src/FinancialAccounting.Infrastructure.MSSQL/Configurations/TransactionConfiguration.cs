using FinancialAccounting.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.
                HasKey(t => t.Id);

            builder.
                HasOne(t => t.Account).
                WithMany().
                HasForeignKey(t => t.AccountId).
                IsRequired().
                OnDelete(DeleteBehavior.Cascade);

            builder.
                HasOne(t => t.Category).
                WithMany().
                HasForeignKey(t => t.CategoryId).
                OnDelete(DeleteBehavior.Restrict).
                IsRequired(false);


            builder.
                Property(t => t.Value).
                HasPrecision(18, 2).
                IsRequired();

            builder.
                Property(t => t.CreatedTime).
                HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.
                Property(t => t.Type).
                HasConversion<int>();

            builder.
                Property(t => t.Description).
                HasMaxLength(100).
                IsRequired(false);
        }
    }
}
