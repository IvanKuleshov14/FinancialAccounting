
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
    }
}
