using FinancialAccouting.Contracts.Targets;
using FluentValidation;

namespace FinancialAccounting.Application.Targets.Validators
{
    public class CreateTargetValidator : AbstractValidator<CreateTargetDto>
    {
        public CreateTargetValidator()
        {
            RuleFor(x => x.userId).
                NotEmpty().WithMessage("Id пользователя не может быть пустым");

            RuleFor(x => x.name).
                NotEmpty().WithMessage("Название цели не указано").
                MaximumLength(30).WithMessage("Название цели не должно превышать 30 символов");

            RuleFor(x => x.total).
                GreaterThanOrEqualTo(0).WithMessage("Сумма не может быть меньше 0");

            RuleFor(x => x.goal).
                GreaterThanOrEqualTo(0).WithMessage("Цель не может быть меньше 0");
        }
    }
}
