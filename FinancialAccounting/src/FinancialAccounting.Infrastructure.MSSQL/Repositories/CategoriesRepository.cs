using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Entities.Categories;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        public Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
