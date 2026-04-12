using FinancialAccouting.Contracts;
using FluentValidation;

namespace FinancialAccounting.Application.Accounts
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
    { 
        public CreateAccountValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Название счета не может быть пустым").
                MaximumLength(30).WithMessage("Название счета не может быть длиннее 30 символов");

            RuleFor(x => x.UserId).
                NotEmpty().WithMessage("Id пользователя не может быть пустым");

            RuleFor(x => x.Total).
                GreaterThanOrEqualTo(0).WithMessage("На счете не может быть отрицательного баланса");
        }
    }
}
