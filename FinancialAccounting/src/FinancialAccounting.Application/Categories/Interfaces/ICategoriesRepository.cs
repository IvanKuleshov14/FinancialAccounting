using FinancialAccounting.Entities.Categories;

namespace FinancialAccounting.Application.Categories.Interfaces
{
    public interface ICategoriesRepository
    {
        Task AddAsync(Category category, CancellationToken cancellationToken);
        Task UpdateAsync(Guid categoryId, string categoryName, CancellationToken cancellationToken);
        Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken);
        Task<List<Category>> GetCategoriesByTypeAsync(CategoryTypes type, CancellationToken cancellationToken);
    }
}
