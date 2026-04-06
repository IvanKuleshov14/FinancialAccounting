using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Entities.Categories;
using FinancialAccounting.Infrastructure.MSSQL.Data;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public CategoriesRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid categoryId, string categoryName, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if(category == null)
            {
                throw new Exception("Категория не найдена");
            }
            
            category.Name = categoryName;

            await _dbContext.SaveChangesAsync();
        }
    }
}
