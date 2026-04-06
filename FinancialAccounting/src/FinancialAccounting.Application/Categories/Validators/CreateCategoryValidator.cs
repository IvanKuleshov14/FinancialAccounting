using FinancialAccouting.Contracts;
using FinancialAccouting.Contracts.Categories;
using FluentValidation;

namespace FinancialAccounting.Application.Categories.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Имя категории не может быть пустым").
                MaximumLength(30).WithMessage("Название категории не должно превышать 30-ти символов");

            RuleFor(x => x.Type).
                NotEmpty().WithMessage("Тип категории не указан");
        }
    }
}
