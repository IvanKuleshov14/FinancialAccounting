using FinancialAccounting.Entities.Transactions;
using FinancialAccounting.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using FinancialAccounting.Entities.Categories;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.
                HasKey(t => t.Id);

            builder.
                HasOne<Account>().
                WithMany().
                HasForeignKey(t => t.AccountId).
                IsRequired().
                OnDelete(DeleteBehavior.Cascade);

            builder.
                HasOne<Category>().
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

            //builder.
            //    Property(t => t.CreatedDay).
            //    IsRequired();

            builder.
                Property(t => t.Type).
                HasConversion<int>();
        }
    }
}
