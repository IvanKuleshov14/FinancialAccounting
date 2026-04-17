using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialAccounting.Infrastructure.MSSQL.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.
                HasKey(a => a.Id);

            // Для дальнейшей авторизации
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
                HasPrecision(18, 2).
                HasDefaultValue(0);
        }
    }
}
