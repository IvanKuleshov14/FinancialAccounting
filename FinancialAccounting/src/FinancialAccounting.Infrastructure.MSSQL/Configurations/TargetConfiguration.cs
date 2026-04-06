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
                HasColumnType("decimal(18,2)").
                HasDefaultValue(0);

            builder.
                Property(t => t.Goal).
                HasColumnType("decimal(18,2)").
                IsRequired();
        }
    }
}
