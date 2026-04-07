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
        public TargetsService(
            ITargetsRepository targetsRepository,
            IValidator<CreateTargetDto> createTargetValidator)
        {
            _targetsRepository = targetsRepository;
            _createValidator = createTargetValidator;
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
    }
}
