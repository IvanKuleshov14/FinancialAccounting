using FinancialAccounting.Entities.Categories;
using FinancialAccounting.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.
                HasKey(c => c.Id);

            builder.
                Property(c => c.Name).
                IsRequired().
                HasMaxLength(30);

            builder.
                Property(c => c.Type).
                HasConversion<int>();

            builder.
                HasMany<Transaction>().
                WithOne().
                HasForeignKey(t => t.CategoryId).
                OnDelete(DeleteBehavior.Restrict);
        }
    }
}
