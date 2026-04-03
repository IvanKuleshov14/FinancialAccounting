using FinancialAccouting.Contracts.Categories;

namespace FinancialAccounting.Application.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task AddAsync(CreateCategoryDto categoryDto, CancellationToken cancellationToken);
    }
}
