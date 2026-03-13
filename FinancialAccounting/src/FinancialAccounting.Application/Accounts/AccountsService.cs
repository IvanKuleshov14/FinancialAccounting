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
        private readonly IValidator<CreateAccountDto> _validator;
        private readonly ILogger<AccountsService> _logger;

        public AccountsService(IAccountsRepository accountsRepository, IValidator<CreateAccountDto> validator, ILogger<AccountsService> logger)
        {
            _accountsRepository = accountsRepository;
            _validator = validator;
            _logger = logger;
        }


        public async Task Create(
            CreateAccountDto accountDto,
            CancellationToken cancellationToken)
        {
            // Валидация
            var validatorResult = await _validator.ValidateAsync(accountDto, cancellationToken);
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

            // Логирование об успешном или неуспешном сохранении

            _logger.LogInformation("Account Created with id {accountId}", accountId);
        }
    }
}
