using FinancialAccouting.Contracts.TargetTransactions;
using FluentValidation;

namespace FinancialAccounting.Application.TargetTransactions.Validators
{
    public class CreateTargetTransactionValidator : AbstractValidator<CreateTargetTransactionDto>
    {
        public CreateTargetTransactionValidator()
        {
            RuleFor(x => x.targetId).
                NotEmpty().WithMessage("Не указан id счета");

            RuleFor(x => x.type).
                NotEmpty().WithMessage("Не указан тип транзации");

            RuleFor(x => x.value).
                NotEmpty().WithMessage("Не указана сумма транзакции").
                GreaterThanOrEqualTo(0).WithMessage("Сумма транзакции должна быть >= 0");

            RuleFor(x => x.createdDay).
                NotEmpty().WithMessage("Дата транзакции не указана или указана некорректно");

            RuleFor(x => x.description).
                MaximumLength(100).WithMessage("Описание не должно превышать 100 символов");
        }
    }
}
