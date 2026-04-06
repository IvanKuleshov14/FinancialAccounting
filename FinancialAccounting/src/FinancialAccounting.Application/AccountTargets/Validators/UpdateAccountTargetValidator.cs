using FinancialAccouting.Contracts;
using FinancialAccouting.Contracts.AccountTargets;
using FluentValidation;

namespace FinancialAccounting.Application.AccountTargets.Validators
{
    public class UpdateAccountTargetValidator : AbstractValidator<UpdateAccountTargetDto>
    {
        public UpdateAccountTargetValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Название счета не может быть пустым").
                MaximumLength(30).WithMessage("Название счета не может быть длиннее 30 символов");

            RuleFor(x => x.Goal).
                GreaterThanOrEqualTo(0).WithMessage("Цель не может быть меньше, чем 0");
        }
    }
}
