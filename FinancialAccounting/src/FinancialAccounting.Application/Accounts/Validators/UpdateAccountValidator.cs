using FinancialAccouting.Contracts;
using FluentValidation;

namespace FinancialAccounting.Application.Accounts
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountDto>
    {
        public UpdateAccountValidator()
        {
            RuleFor(x => x.Name).
                NotEmpty().WithMessage("Название счета не может быть пустым").
                MaximumLength(30).WithMessage("Название счета не может быть длиннее 30 символов");
        }
    }
}
