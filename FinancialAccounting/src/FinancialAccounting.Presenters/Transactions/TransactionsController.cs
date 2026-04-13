using FinancialAccounting.Application.Transactions;
using FinancialAccouting.Contracts.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.Transactions
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionService;

        public TransactionsController(ITransactionsService transactionsService)
        {
            _transactionService = transactionsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransactionDto request,
            CancellationToken cancellationToken)
        {
            await _transactionService.Create(request, cancellationToken);
            return Ok("Transaction created");
        }

        [HttpPost("transfers")]
        public async Task<IActionResult> CreateTransfer(
            [FromBody] CreateTransferDto request,
            CancellationToken cancellationToken)
        {
            await _transactionService.CreateTransfer(request, cancellationToken);
            return Ok("Transfer created");
        }

        [HttpDelete("{transactionId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid transactionId,
            CancellationToken cancellationToken)
        {
            await _transactionService.Delete(transactionId, cancellationToken);
            return Ok("Transaction deleted");
        }

        [HttpGet]
        public async Task<IEnumerable<GetTransactionListDto>> GetTransactions(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var result = await _transactionService.GetTransactions(page, limit, cancellationToken);
            return result;
        }

        [HttpGet("{accountId:guid}")]
        public async Task<IEnumerable<GetTransactionListDto>> GetTransactionsByAccountId(
            CancellationToken cancellationToken,
            [FromRoute] Guid accountId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
            )
        {
            var result = await _transactionService.GetTransactionsByAccountId(accountId, page, limit, cancellationToken);
            return result;
        }
    }
}
