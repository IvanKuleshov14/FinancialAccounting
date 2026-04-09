using FinancialAccounting.Application.Targets.Interfaces;
using FinancialAccounting.Application.Targets.Validators;
using FinancialAccounting.Entities.Targets;
using FinancialAccouting.Contracts.Targets;
using FluentValidation;

namespace FinancialAccounting.Application.Targets
{
    public class TargetsService : ITargetsService
    {
        private readonly ITargetsRepository _targetsRepository;
        private readonly IValidator<CreateTargetDto> _createValidator;
        private readonly IValidator<UpdateTargetDto> _updateValidator;
        public TargetsService(
            ITargetsRepository targetsRepository,
            IValidator<CreateTargetDto> createTargetValidator,
            IValidator<UpdateTargetDto> updateValidator)
        {
            _targetsRepository = targetsRepository;
            _createValidator = createTargetValidator;
            _updateValidator = updateValidator;
        }

        public async Task Create(CreateTargetDto targetDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createValidator.ValidateAsync(targetDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var targetId = Guid.NewGuid();
            var target = new Target(
                targetId,
                targetDto.userId,
                targetDto.name,
                targetDto.total,
                targetDto.goal
                );

            await _targetsRepository.AddAsync(target, cancellationToken);
        }

        public async Task Update(Guid targetId, UpdateTargetDto updateTargetDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _updateValidator.ValidateAsync(updateTargetDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var newTargetName = updateTargetDto.name;
            var newTargetGoal = updateTargetDto.goal;

            await _targetsRepository.UpdateAsync(targetId, newTargetName, newTargetGoal, cancellationToken);
        }

        public async Task Delete(Guid targetId, CancellationToken cancellationToken)
        {
            await _targetsRepository.DeleteAsync(targetId, cancellationToken);
        }
    }
}
