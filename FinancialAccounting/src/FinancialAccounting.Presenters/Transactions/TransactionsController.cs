using FinancialAccounting.Application.Transactions;
using FinancialAccouting.Contracts.Transactions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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

        [HttpDelete("{transactionId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid transactionId,
            CancellationToken cancellationToken)
        {
            await _transactionService.Delete(transactionId, cancellationToken);
            return Ok("Transaction deleted");
        }

    }
}
