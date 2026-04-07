using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Application.AccountTargets.Interfaces;
using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Application.Targets.Interfaces;
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
            services.AddScoped<IAccountTargetsRepository, AccountTargetsRepository>();
            services.AddScoped<ITransactionsRepository, TransactionsRepository>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<ITargetsRepository, TargetsRepository>();

            return services;
        }
    }
}
