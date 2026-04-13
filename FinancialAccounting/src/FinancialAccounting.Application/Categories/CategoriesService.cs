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
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidotator;
        public CategoriesService(
            ICategoriesRepository categoriesRepository,
            IValidator<CreateCategoryDto> createCategoryValidator,
            IValidator<UpdateCategoryDto> updateCategoryValidotator)
        {
            _categoriesRepository = categoriesRepository;
            _createCategoryValidotator = createCategoryValidator;
            _updateCategoryValidotator = updateCategoryValidotator;
        }


        public async Task Create(CreateCategoryDto categoryDto, CancellationToken cancellationToken)
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

        public async Task Update(Guid categoryId, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _updateCategoryValidotator.ValidateAsync(updateCategoryDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var newCategoryName = updateCategoryDto.Name;

            await _categoriesRepository.UpdateAsync(categoryId, newCategoryName, cancellationToken);
        }

        public async Task Delete(Guid categoryId, CancellationToken cancellationToken)
        {
            await _categoriesRepository.DeleteAsync(categoryId, cancellationToken);
        }

        public async Task<List<GetCategoryListDto>> GetCategoriesByType(CategoryTypes type, CancellationToken cancellationToken)
        {
            var categories = await _categoriesRepository.GetCategoriesByTypeAsync(type, cancellationToken);

            return categories.Select(category => new GetCategoryListDto(
                category.Id,
                category.Name,
                (int)category.Type
                )).ToList();
        }
    }
}
