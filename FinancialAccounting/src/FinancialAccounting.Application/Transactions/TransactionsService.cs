using FinancialAccouting.Contracts.Transactions;
using FinancialAccounting.Entities.Transactions;
using FluentValidation;



namespace FinancialAccounting.Application.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IValidator<CreateTransactionDto> _createValidator;
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository, IValidator<CreateTransactionDto> createValidator)
        {
            _transactionsRepository = transactionsRepository;
            _createValidator = createValidator;
        }


        public async Task Create(CreateTransactionDto transactionDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createValidator.ValidateAsync(transactionDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var transactionId = Guid.NewGuid();
            var transaction = new Transaction(
                transactionId,
                transactionDto.AccountId,
                transactionDto.TransactionType,
                transactionDto.Value,
                transactionDto.CreatedDay,
                transactionDto.Description
                );

            await _transactionsRepository.AddAsync(transaction, cancellationToken);
        }
    }
}
