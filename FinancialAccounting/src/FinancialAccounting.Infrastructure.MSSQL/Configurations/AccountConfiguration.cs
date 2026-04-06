using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.
                HasKey(a => a.Id);

            //builder.
            //    HasOne<User>().
            //    WithMany().
            //    HasForeignKey(a => a.UserId).
            //    IsRequired().
            //    OnDelete(DeleteBehavior.Cascade);

            builder.
                HasIndex(a => a.UserId);

            builder.
                Property(a => a.Name).
                IsRequired().
                HasMaxLength(30);

            builder.
                Property(a => a.Total).
                HasColumnType("decimal(18,2)").
                HasDefaultValue(0);
        }
    }
}
