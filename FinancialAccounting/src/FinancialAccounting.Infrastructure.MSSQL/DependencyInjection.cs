using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Application.Transactions;
using FinancialAccounting.Infrastructure.MSSQL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAccounting.Infrastructure.MSSQL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInsfrasturcture(this IServiceCollection services)
        {
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<ITransactionsRepository, TransactionsRepository>();

            return services;
        }
    }
}
