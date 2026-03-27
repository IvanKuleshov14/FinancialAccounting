using FinancialAccouting.Contracts.Transactions;
using FluentValidation;

namespace FinancialAccounting.Application.Transactions.Validators
{
    public class CreateTransferValidator : AbstractValidator<CreateTransferDto>
    {
        public CreateTransferValidator()
        {
            RuleFor(x => x.FromAccountId).
                NotEmpty().WithMessage("Не указан id счета для списания");

            RuleFor(x => x.ToAccountId).
                NotEmpty().WithMessage("Не указан id счета для пополнения");

            RuleFor(x => x.Value).
                NotEmpty().WithMessage("Не указана сумма транзакции").
                GreaterThanOrEqualTo(0).WithMessage("Сумма транзакции должна быть >= 0");

            RuleFor(x => x.CreatedDay).
                NotEmpty().WithMessage("Дата транзакции не указана или указана некорректно");

            RuleFor(x => x.Description).
                MaximumLength(100).WithMessage("Описание не должно превышать 100 символов");
        }
    }
}
