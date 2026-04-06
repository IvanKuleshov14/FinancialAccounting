using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Application.Categories.Validators;
using FinancialAccounting.Entities.Categories;
using FinancialAccouting.Contracts.Categories;
using FluentValidation;

namespace FinancialAccounting.Application.Categories
{
    public class CategoriesService : ICategoryService
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidotator;
        public CategoriesService(
            ICategoriesRepository categoriesRepository,
            IValidator<CreateCategoryDto> createCategoryValidator)
        {
            _categoriesRepository = categoriesRepository;
            _createCategoryValidotator = createCategoryValidator;
        }


        public async Task AddAsync(CreateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createCategoryValidotator.ValidateAsync(categoryDto);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var categoryId = Guid.NewGuid();
            var category = new Category(
                categoryId,
                categoryDto.Name,
                categoryDto.Type
                );
            
            await _categoriesRepository.AddAsync(category, cancellationToken);
        }
    }
}
