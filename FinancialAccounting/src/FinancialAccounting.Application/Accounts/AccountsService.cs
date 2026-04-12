using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Entities.Accounts;
using FinancialAccouting.Contracts;
using FinancialAccouting.Contracts.Accounts;
using FluentValidation;
using System.Runtime.CompilerServices;

namespace FinancialAccounting.Application
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IValidator<CreateAccountDto> _createValidator;
        private readonly IValidator<UpdateAccountDto> _updateValidator;
        private readonly IValidator<CreateAccountTargetDto> _createAccountTargetValidator;

        public AccountsService(
            IAccountsRepository accountsRepository,
            IValidator<CreateAccountDto> createValidator,
            IValidator<UpdateAccountDto> updateValidator,
            IValidator<CreateAccountTargetDto> createAccountTargetValidator)
        {
            _accountsRepository = accountsRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _createAccountTargetValidator = createAccountTargetValidator;
        }


        public async Task Create(CreateAccountDto accountDto, CancellationToken cancellationToken)
        {
            // Валидация
            var validatorResult = await _createValidator.ValidateAsync(accountDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            // Создание счета
            var accountId = Guid.NewGuid();
            var account = new Account(
                accountId,
                accountDto.Name,
                accountDto.UserId,
                accountDto.Total
                );

            // Запись в базу даных
            await _accountsRepository.AddAsync(account, cancellationToken);
        }

        public async Task Update(Guid accountId, UpdateAccountDto accountDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _updateValidator.ValidateAsync(accountDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var newAccountName = accountDto.Name;

            await _accountsRepository.UpdateAsync(accountId, newAccountName, cancellationToken);
        }

        public async Task Delete(Guid accountId, CancellationToken cancellationToken)
        {
            await _accountsRepository.DeleteAsync(accountId, cancellationToken);
        }

        public async Task CreateTarget(Guid accountId, CreateAccountTargetDto accountTargetDto, CancellationToken cancellationToken)
        {
            var validatorResult = await _createAccountTargetValidator.ValidateAsync(accountTargetDto, cancellationToken);
            if (!validatorResult.IsValid)
            {
                throw new ValidationException(validatorResult.Errors);
            }

            var accountTargetId = Guid.NewGuid();
            var accountTarget = new AccountTarget(
                accountTargetId,
                accountId,
                accountTargetDto.Name,
                accountTargetDto.Goal
                );

            await _accountsRepository.AddTargetAsync(accountTarget, cancellationToken);
        }

        public async Task<GetAccountDto?> GetAccount(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountsRepository.GetAccountByIdAsync(id, cancellationToken);
            if (account == null)
            {
                return null;
            }

            decimal? progress = null;
            if (account.Target != null)
            {
                progress = Math.Round(account.Total / account.Target.Goal * 100, 2);
            }

            return new GetAccountDto(
                account.Id,
                account.Name,
                account.Total,
                account.Target?.Name,
                account.Target?.Goal,
                progress
                );
        }
    }
}
