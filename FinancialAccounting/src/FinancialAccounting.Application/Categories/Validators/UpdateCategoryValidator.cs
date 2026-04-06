using FinancialAccouting.Contracts.Categories;
using FluentValidation;

namespace FinancialAccounting.Application.Categories.Validators
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Имя категории не может быть пустым").
                MaximumLength(30).WithMessage("Название категории не должно превышать 30-ти символов");
        }
    }
}
