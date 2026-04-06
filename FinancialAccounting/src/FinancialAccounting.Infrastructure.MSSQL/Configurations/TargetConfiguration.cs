using FinancialAccounting.Entities.Targets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class TargetConfiguration : IEntityTypeConfiguration<Target>
    {
        public void Configure(EntityTypeBuilder<Target> builder)
        {
            builder.
                HasKey(t => t.Id);

            builder.
                Property(t => t.Name).
                IsRequired().
                HasMaxLength(30);

            builder.
                Property(t => t.Total).
                HasPrecision(18, 2).
                HasDefaultValue(0);

            builder.
                Property(t => t.Goal).
                HasPrecision(18, 2).
                IsRequired();
        }
    }
}
