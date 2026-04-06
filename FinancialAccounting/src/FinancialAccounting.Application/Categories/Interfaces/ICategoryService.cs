using FinancialAccouting.Contracts.Categories;

namespace FinancialAccounting.Application.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task Create(CreateCategoryDto categoryDto, CancellationToken cancellationToken);
        Task Update(Guid categoryId, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken);
        Task Delete(Guid categoryId, CancellationToken cancellationToken);
    }
}
