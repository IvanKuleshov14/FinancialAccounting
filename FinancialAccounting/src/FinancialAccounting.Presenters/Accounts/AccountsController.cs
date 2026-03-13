using FinancialAccounting.Application;
using FinancialAccouting.Contracts;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{accountId:guid}")]
        public async Task<IActionResult> Get(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return Ok("Account geted");
        }

        [HttpDelete("{accountId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return Ok("Account deleted");
        }

        [HttpPut("{accountId:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid accountId,
            [FromBody] UpdateAccountDto request,
            CancellationToken cancellationToken)
        {
            return Ok("Account updated");
        }

        [HttpPost("{accountId:guid}/account_targets")]
        public async Task<IActionResult> CreateAccountTarget(
            [FromRoute] Guid accountId,
            [FromBody] CreateAccountTargetDto request,
            CancellationToken cancellationToken)
        {
            return Ok("AccountTarget created");
        }
    }
}
