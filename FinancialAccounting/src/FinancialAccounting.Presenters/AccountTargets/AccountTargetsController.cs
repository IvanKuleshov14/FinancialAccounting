using FinancialAccounting.Application.AccountTargets.Interfaces;
using FinancialAccouting.Contracts.AccountTargets;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.AccountTargets
{
    [ApiController]
    [Route("[controller]")]
    public class AccountTargetsController : ControllerBase
    {
        private readonly IAccountTargetsService _accountTargetsService;
        public AccountTargetsController(IAccountTargetsService accountTargetsService)
        {
            _accountTargetsService = accountTargetsService;
        }


        [HttpPut("{accountTargetId:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid accountTargetId,
            [FromBody] UpdateAccountTargetDto request,
            CancellationToken cancellationToken
            )
        {
            await _accountTargetsService.Update(accountTargetId, request, cancellationToken);
            return Ok("Target updated");
        }

        [HttpDelete("{accountTargetId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid accountTargetId,
            CancellationToken cancellationToken
            )
        {
            await _accountTargetsService.Delete(accountTargetId, cancellationToken);
            return Ok("Target deleted");
        }
    }
}
