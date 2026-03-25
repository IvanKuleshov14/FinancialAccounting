using FinancialAccounting.Application;
using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Infrastructure.MSSQL.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Infrastructure.MSSQL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInsfrasturcture(this IServiceCollection services)
        {
            //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddScoped<IAccountsRepository, AccountsRepository>();

            return services;
        }
    }
}
