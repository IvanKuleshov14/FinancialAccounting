using FinancialAccouting.Contracts.Categories;

namespace FinancialAccounting.Application.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task Create(CreateCategoryDto categoryDto, CancellationToken cancellationToken);
        Task Update(Guid categoryId, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken);
        Task Delete(Guid categoryId, CancellationToken cancellationToken);
        Task<List<GetCategoryListDto>> GetCategoriesByType(CategoryTypes type, CancellationToken cancellationToken);
    }
}
