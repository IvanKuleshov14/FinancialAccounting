using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccouting.Contracts.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounting.Presenters.Categories
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryDto request,
            CancellationToken cancellationToken
            )
        {
            await _categoryService.AddAsync(request, cancellationToken);
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
