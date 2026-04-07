using FinancialAccounting.Application.Targets.Interfaces;
using FinancialAccouting.Contracts.Targets;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.Targets
{
    [ApiController]
    [Route("[controller]")]
    public class TargetsController : ControllerBase
    {
        private readonly ITargetsService _targetsService;
        public TargetsController(ITargetsService targetsService)
        {
            _targetsService = targetsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTargetDto request,
            CancellationToken cancellationToken
            )
        {
            await _targetsService.Create(request, cancellationToken);
            return Ok("Target created");
        }
    }
}
