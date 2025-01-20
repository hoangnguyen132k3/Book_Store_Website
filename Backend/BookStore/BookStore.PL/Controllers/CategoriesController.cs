using BookStore.BLL.Services;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookStore.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoresController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoresController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all-categories")]
        public async Task<ActionResult<IEnumerable<CategoryVm>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoryAsync();
            return Ok(result); 
        }

        [HttpGet("get-all-categories-by-page")]
        public async Task<ActionResult<IEnumerable<CategoryVm>>> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetAllCategoryAsync(pageNumber, pageSize);
            return Ok(result); 
        }

        [HttpGet("get-category-by-id/{id}")]
        public async Task<ActionResult<CategoryVm>> GetCategory(int id)
        {
            var result = await _categoryService.GetByCategoryIdAsync(id);

            if (result == null)
            {
                return NotFound(new { Message = $"Category với ID {id} không tìm thấy." }); 
            }

            return Ok(result); 
        }

        [HttpPost("add-new-category")]
        public async Task<ActionResult<Category>> PostCategory(InputCategoryVm categoryVm)
        {
            var result = await _categoryService.AddCategoryAsync(categoryVm);

            if (result == null)
            {
                return BadRequest(new { Message = "Không tạo được category." }); 
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result); 
        }

        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> PutCategory(int id, InputCategoryVm categoryVm)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryVm);
            if (result == null)
            {
                return NotFound(new { Message = $"Category nới ID {id} không tìm thấy." }); 
            }

            return Ok(result);
        }

        [HttpDelete("delete-category-by-id/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Category với ID {id} không tìm thấy." }); 
            }

            return NoContent();
        }
        

    }
}
