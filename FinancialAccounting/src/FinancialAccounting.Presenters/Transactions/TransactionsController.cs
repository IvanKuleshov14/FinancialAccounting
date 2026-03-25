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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateTransactionDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
