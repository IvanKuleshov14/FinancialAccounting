using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Application.Categories;
using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Application.Transactions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddScoped<IAccountsService, AccountsService>();
            services.AddScoped<ITransactionsService, TransactionsService>();
            services.AddScoped<ICategoryService, CategoriesService>();

            return services;
        }
    }
}
