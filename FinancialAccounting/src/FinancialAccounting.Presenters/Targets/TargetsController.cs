using FinancialAccounting.Application.Targets.Interfaces;
using FinancialAccounting.Entities.Targets;
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

        [HttpPut("{targetId:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid targetId,
            [FromBody] UpdateTargetDto request,
            CancellationToken cancellationToken
            )
        {
            await _targetsService.Update(targetId, request, cancellationToken);
            return Ok("Target updated");
        }

        [HttpDelete("{targetId:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid targetId,
            CancellationToken cancellationToken
            )
        {
            await _targetsService.Delete(targetId, cancellationToken);
            return Ok("Target deleted");
        }

        [HttpGet]
        public async Task<IEnumerable<GetTargetDto>> GetAllTargets(
            CancellationToken cancellationToken
            )
        {
            var result = await _targetsService.GetAllTargets(cancellationToken);
            return result;
        }
    }
}
