using FinancialAccouting.Contracts.Transactions;
using FinancialAccounting.Entities.Transactions;
using FluentValidation;



namespace FinancialAccounting.Application.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IValidator<CreateTransactionDto> _createValidator;
        private readonly IValidator<CreateTransferDto> _createTransferValidator;
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository, IValidator<CreateTransactionDto> createValidator, IValidator<CreateTransferDto> createTransferValidator)
        {
            _transactionsRepository = transactionsRepository;
            _createValidator = createValidator;
            _createTransferValidator = createTransferValidator;
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
                transactionDto.Description,
                null
                );

            await _transactionsRepository.AddAsync(transaction, cancellationToken);
        }

        public async Task CreateTransfer(CreateTransferDto transferDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createTransferValidator.ValidateAsync(transferDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var linkId = Guid.NewGuid();

            var transactionExpenseId = Guid.NewGuid();
            var transactionExpense = new Transaction(
                transactionExpenseId,
                transferDto.FromAccountId,
                TransactionTypes.Expense,
                transferDto.Value,
                transferDto.CreatedDay,
                transferDto.Description,
                linkId
                );

            var transactionIncomeId = Guid.NewGuid();
            var transactionIncome = new Transaction(
                transactionIncomeId,
                transferDto.ToAccountId,
                TransactionTypes.Income,
                transferDto.Value,
                transferDto.CreatedDay,
                transferDto.Description,
                linkId
                );

            await _transactionsRepository.AddTransferAsync(transactionExpense, transactionIncome, cancellationToken);
        }

        public async Task Delete(Guid transactionId, CancellationToken cancellationToken)
        {
            await _transactionsRepository.DeleteAsync(transactionId, cancellationToken);
        }
    }
}
