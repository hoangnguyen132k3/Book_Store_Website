using BookStore.BLL.Services;
using BookStore.BLL.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;

namespace BookStore.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("get-all-books")]
        public async Task<ActionResult<IEnumerable<BookVm>>> GetBooks()
        {
            var result = await _bookService.GetAllBookAsync();
            return Ok(result); 
        }

        [HttpGet("search-by-name")]
        public async Task<ActionResult<IEnumerable<BookVm>>> GetBooksByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var result = await _bookService.GetBooksByNameAsync(name);
            return Ok(result);
        }


        

        [HttpGet("get-book-by-id/{id}")]
        public async Task<ActionResult<BookVm>> GetBook(int id)
        {
            var result = await _bookService.GetByBookIdAsync(id);

            if (result == null)
            {
                return NotFound(new { Message = $"Sách với ID {id} không tìm thấy." }); 
            }

            return Ok(result); 
        }
        [HttpGet("get-book-by-category/{categoryId}")]
        public async Task<IActionResult> GetBooksByCategory(int categoryId)
        {
            var books = await _bookService.GetBooksByCategoryAsync(categoryId);

            if (books == null || !books.Any())
            {
                return NotFound(new { Message = "Không tìm thấy sách nào cho danh mục này." });
            }

            return Ok(books);
        }
        [HttpPost("add-new-book")]
        public async Task<ActionResult<BookVm>> PostBook(InputBookVm bookVm)
        {
            var result = await _bookService.AddBookAsync(bookVm);

            if (result == null)
            {
                return BadRequest(new { Message = "Không tạo được sách." }); 
            }

            return CreatedAtAction(nameof(GetBook), new { id = result.BookId }, result); 
        }

        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> PutBook(int id, InputBookVm bookVm)
        {
            var result = await _bookService.UpdateBookAsync(id, bookVm);
            if (result == null)
            {
                return NotFound(new { Message = $"Sách với ID {id} không tìm thấy." });
            }

            return Ok(result);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Sách với ID  {id}  không tìm thấy." }); 
            }

            return NoContent(); 
        }
        
        

    }
}
