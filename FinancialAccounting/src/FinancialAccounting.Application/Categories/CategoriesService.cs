using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Application.Categories.Validators;
using FinancialAccouting.Contracts.Categories;
using FluentValidation;

namespace FinancialAccounting.Application.Categories
{
    public class CategoriesService : ICategoryService
    {
        private readonly IValidator<CreateCategoryDto> _createCategoryValidotator;
        public CategoriesService(
            IValidator<CreateCategoryDto> createCategoryValidator)
        {
            _createCategoryValidotator = createCategoryValidator;
        }


        public Task AddAsync(CreateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
