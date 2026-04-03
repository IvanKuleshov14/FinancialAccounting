using FinancialAccouting.Contracts.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.Categories
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryDto request,
            CancellationToken cancellation
            )
        {
            return Ok("Category created");
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            return Ok("Category updated");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            return Ok("Category deleted");
        }
    }
}
