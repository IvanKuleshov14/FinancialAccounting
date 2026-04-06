using FinancialAccounting.Application.AccountTargets.Interfaces;
using FinancialAccouting.Contracts.AccountTargets;
using FluentValidation;

namespace FinancialAccounting.Application.AccountTargets
{
    public class AccountTargetsService : IAccountTargetsService
    {
        private readonly IAccountTargetsRepository _accountTargetsRepository;
        private readonly IValidator<UpdateAccountTargetDto> _updateAccountTargetValidator;
        public AccountTargetsService(
            IAccountTargetsRepository accountTargetsRepository,
            IValidator<UpdateAccountTargetDto> updateAccountTargetValidator)
        {
            _accountTargetsRepository = accountTargetsRepository;
            _updateAccountTargetValidator = updateAccountTargetValidator;
        }


        public async Task Update(Guid accountTargetId, UpdateAccountTargetDto updateAccountTargetDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _updateAccountTargetValidator.ValidateAsync(updateAccountTargetDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var newAccountTargetName = updateAccountTargetDto.Name;
            var newAccountTargetGoal = updateAccountTargetDto.Goal;

            await _accountTargetsRepository.UpdateAsync(accountTargetId, newAccountTargetName, newAccountTargetGoal, cancellationToken);
        }
    }
}
