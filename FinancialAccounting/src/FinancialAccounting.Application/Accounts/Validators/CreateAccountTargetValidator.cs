using FinancialAccouting.Contracts;
using FluentValidation;

namespace FinancialAccounting.Application.Accounts
{
    public class CreateAccountTargetValidator : AbstractValidator<CreateAccountTargetDto>
    {
        public CreateAccountTargetValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Название счета не может быть пустым").
                MaximumLength(30).WithMessage("Название счета не может быть длиннее 30 символов");

            RuleFor(x => x.Goal).
                GreaterThanOrEqualTo(0).WithMessage("Цель не может быть меньше, чем 0");
        }
    }
}
