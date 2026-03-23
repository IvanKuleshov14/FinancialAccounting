using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Entities.Account;
using FinancialAccouting.Contracts;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FinancialAccounting.Application
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IValidator<CreateAccountDto> _createValidator;
        private readonly IValidator<UpdateAccountDto> _updateValidator;

        public AccountsService(
            IAccountsRepository accountsRepository,
            IValidator<CreateAccountDto> createValidator,
            IValidator<UpdateAccountDto> updateValidator)
        {
            _accountsRepository = accountsRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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
    }
}
