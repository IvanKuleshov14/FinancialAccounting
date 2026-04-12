using FinancialAccounting.Entities.TargetTransactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinancialAccounting.Entities.Targets;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class TargetTransactionConfiguration : IEntityTypeConfiguration<TargetTransaction>
    {
        public void Configure(EntityTypeBuilder<TargetTransaction> builder)
        {
            builder.
                HasKey(t => t.Id);

            builder.
                HasOne<Target>().
                WithMany().
                HasForeignKey(t => t.TargetId).
                IsRequired().
                OnDelete(DeleteBehavior.Cascade);

            builder.
                Property(t => t.Value).
                HasPrecision(18, 2).
                IsRequired();

            builder.
                Property(t => t.CreatedTime).
                HasDefaultValueSql("CURRENT_TIMESTAMP");

            //builder.
            //    Property(t => t.CreatedDay).
            //    IsRequired();

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
