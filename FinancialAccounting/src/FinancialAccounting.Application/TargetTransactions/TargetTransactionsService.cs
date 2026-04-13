using FinancialAccounting.Application.TargetTransactions.Interfaces;
using FinancialAccounting.Entities.TargetTransactions;
using FinancialAccouting.Contracts.TargetTransactions;
using FluentValidation;
using Newtonsoft.Json.Linq;
using System.Transactions;

namespace FinancialAccounting.Application.TargetTransactions
{
    public class TargetTransactionsService : ITargetTransactionsService
    {
        private readonly ITargetTransactionsRepository _targetTransactionsRepository;
        private readonly IValidator<CreateTargetTransactionDto> _createTargetTransactionValidator;
        public TargetTransactionsService(
            ITargetTransactionsRepository targetTransactionsRepository,
            IValidator<CreateTargetTransactionDto> createTargetTransactionValidator
            )
        {
            _targetTransactionsRepository = targetTransactionsRepository;
            _createTargetTransactionValidator = createTargetTransactionValidator;
        }

        public async Task Create(CreateTargetTransactionDto targetTransactionDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createTargetTransactionValidator.ValidateAsync(targetTransactionDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var targetTransactionId = Guid.NewGuid();
            var targetTransaction = new TargetTransaction(
                targetTransactionId,
                targetTransactionDto.targetId,
                targetTransactionDto.type,
                targetTransactionDto.value,
                targetTransactionDto.createdDay,
                targetTransactionDto.description
                );

            await _targetTransactionsRepository.AddAsync(targetTransaction, cancellationToken);
        }

        public async Task Delete(Guid targetTransactionId, CancellationToken cancellationToken)
        {
            await _targetTransactionsRepository.DeleteAsync(targetTransactionId, cancellationToken);
        }

        public async Task<List<GetTargetTransactionListDto>> GetTargetTransactionsByTargetId(Guid targetId, int page, int limit, CancellationToken cancellationToken)
        {
            var transactions = await _targetTransactionsRepository.GetTargetTransactionByTargetIdAsync(targetId, page, limit, cancellationToken);

            return transactions.Select(transaction => new GetTargetTransactionListDto(
                transaction.Id,
                transaction.Target.Name,
                transaction.Value,
                (int)transaction.Type,
                transaction.Description,
                transaction.CreatedDay,
                transaction.CreatedTime
                )).ToList();
        }
    }
}
