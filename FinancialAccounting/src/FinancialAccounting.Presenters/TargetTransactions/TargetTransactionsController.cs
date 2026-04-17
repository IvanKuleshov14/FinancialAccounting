using FinancialAccounting.Application.TargetTransactions.Interfaces;
using FinancialAccouting.Contracts.TargetTransactions;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.TargetTransactions
{
    [ApiController]
    [Route("[controller]")]
    public class TargetTransactionsController : ControllerBase
    {
        private readonly ITargetTransactionsService _targetTransactionsService;
        public TargetTransactionsController(ITargetTransactionsService targetTransactionsService)
        {
            _targetTransactionsService = targetTransactionsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTargetTransactionDto request,
            CancellationToken cancellationToken
            )
        {
            await _targetTransactionsService.Create(request, cancellationToken);
            return Ok("Target transaction created");
        }

        [HttpDelete("{targetTransactionId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid targetTransactionId,
            CancellationToken cancellationToken
            )
        {
            await _targetTransactionsService.Delete(targetTransactionId, cancellationToken);
            return Ok("Target transaction deleted");
        }

        [HttpGet("{targetId:guid}")]
        public async Task<IEnumerable<GetTargetTransactionListDto>> GetTargetTransactionsByTargetId(
            [FromRoute] Guid targetId,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
            )
        {
            var result = await _targetTransactionsService.GetTargetTransactionsByTargetId(targetId, page, limit, cancellationToken);
            return result;
        }
    }
}
