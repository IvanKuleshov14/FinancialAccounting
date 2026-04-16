using FinancialAccounting.Application;
using FinancialAccouting.Contracts;
using FinancialAccouting.Contracts.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace FinancialAccounting.Presenters
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountService;
        public AccountsController(IAccountsService accountService)
        {
            _accountService = accountService;
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAccountDto request,
            CancellationToken cancellationToken)
        {
            await _accountService.Create(request, cancellationToken);

            return Ok("Account created");
        }

        [HttpPut("{accountId:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid accountId,
            [FromBody] UpdateAccountDto request,
            CancellationToken cancellationToken)
        {
            await _accountService.Update(accountId, request, cancellationToken);
            return Ok("Account updated");
        }

        [HttpDelete("{accountId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            await _accountService.Delete(accountId, cancellationToken);
            return Ok("Account deleted");
        }

        [HttpGet("{accountId:guid}")]
        public async Task<IActionResult> Get(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            var result = await _accountService.GetAccount(accountId, cancellationToken);

            if (result == null)
            {
                return NotFound("Счет не найден");
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IEnumerable<GetAccountDto>> GetAllAccounts(
            CancellationToken cancellationToken)
        {
            var result = await _accountService.GetAllAccounts(cancellationToken);
            return result;
        }

        [HttpPost("{accountId:guid}/account_targets")]
        public async Task<IActionResult> CreateAccountTarget(
            [FromRoute] Guid accountId,
            [FromBody] CreateAccountTargetDto request,
            CancellationToken cancellationToken)
        {
            await _accountService.CreateTarget(accountId, request, cancellationToken);
            return Ok("AccountTarget created");
        }
    }
}
