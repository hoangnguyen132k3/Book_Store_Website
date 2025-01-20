using BookStore.BLL.Services;
using BookStore.BLL.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;

namespace BookStore.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookDetailsController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookDetailsController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("get-book-details/{id}")]
        public async Task<ActionResult<BookDetailVm>> GetBookDetails(int id)
        {
            var bookDetails = await _bookService.GetByIdAsync(id);
            if (bookDetails == null)
            {
                return NotFound($"Sách với ID {id} không tìm thấy.");
            }
            return Ok(bookDetails);
        }

        [HttpPost("add-to-cart/{bookId}")]
        public async Task<IActionResult> AddBookToCart(int bookId, [FromBody] CartVm cartVm)
        {
            var result = await _bookService.AddBookToCartAsync(bookId, cartVm);
            if (result)
            {
                return Ok("Sách đã được thêm vào giỏ hàng.");
            }
            return BadRequest("Không thêm được sách vào giỏ hàng.");
        }
    }
}
