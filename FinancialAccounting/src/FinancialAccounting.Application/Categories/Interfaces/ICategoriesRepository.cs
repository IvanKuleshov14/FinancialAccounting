using FinancialAccounting.Entities.Categories;

namespace FinancialAccounting.Application.Categories.Interfaces
{
    public interface ICategoriesRepository
    {
        Task AddAsync(Category category, CancellationToken cancellationToken);
        Task UpdateAsync(Guid categoryId, string categoryName, CancellationToken cancellationToken);
    }
}
