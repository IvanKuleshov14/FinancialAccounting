using FinancialAccouting.Contracts.Targets;
using FluentValidation;

namespace FinancialAccounting.Application.Targets.Validators
{
    public class UpdateTargetValidator : AbstractValidator<UpdateTargetDto>
    {
        public UpdateTargetValidator()
        {
            RuleFor(x => x.name).
                NotEmpty().WithMessage("Название цели не указано").
                MaximumLength(30).WithMessage("Название цели не должно превышать 30 символов");

            RuleFor(x => x.goal).
                GreaterThanOrEqualTo(0).WithMessage("Цель не может быть меньше 0");
        }
    }
}
