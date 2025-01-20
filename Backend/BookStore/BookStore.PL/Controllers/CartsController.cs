using BookStore.BLL.Services.InterfaceServices;
using BookStore.BLL.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // Lấy tất cả giỏ hàng (chỉ dành cho Admin)
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCarts()
    {
        var carts = await _cartService.GetAllCartAsync();
        if (carts == null || !carts.Any())
        {
            return NotFound(new { message = "Không tìm thấy giỏ hàng.", data = (object)null });
        }
        return Ok(new { message = "Lấy danh sách giỏ hàng thành công.", data = carts });
    }

    // Lấy giỏ hàng của một người dùng
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetCartsByUserId(int userId)
    {
        var carts = await _cartService.GetCartsByUserIdAsync(userId);
        if (carts == null || !carts.Any())
        {
            return NotFound(new { message = $"Không tìm thấy giỏ hàng cho UserId: {userId}.", data = (object)null });
        }
        return Ok(new { message = "Lấy giỏ hàng thành công.", data = carts });
    }

    // Lấy chi tiết một mục trong giỏ hàng của người dùng
    [HttpGet("user/{userId}/book/{bookId}")]
    public async Task<IActionResult> GetCartById(int userId, int bookId)
    {
        var cart = await _cartService.GetByCartIdAsync(userId, bookId);
        if (cart == null)
        {
            return NotFound(new { message = $"Không tìm thấy giỏ hàng với UserId: {userId}, BookId: {bookId}.", data = (object)null });
        }
        return Ok(new { message = "Lấy thông tin giỏ hàng thành công.", data = cart });
    }

    // Thêm một mục vào giỏ hàng
    [HttpPost("add")]
    public async Task<IActionResult> AddCart([FromBody] CartVm cart)
    {
        if (cart == null)
        {
            return BadRequest(new { message = "Dữ liệu giỏ hàng không hợp lệ." });
        }

        try
        {
            var result = await _cartService.AddCartAsync(cart);
            if (result > 0)
            {
                return Ok(new { message = "Thêm giỏ hàng thành công.", cartId = result });
            }
            return BadRequest(new { message = "Lỗi khi thêm giỏ hàng." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Lỗi xử lý: {ex.Message}" });
        }
    }

    // Cập nhật số lượng hoặc thông tin một mục trong giỏ hàng
    [HttpPut("user/{userId}/book/{bookId}")]
    public async Task<IActionResult> UpdateCart(int userId, int bookId, [FromBody] CartVm cartVm)
    {
        if (cartVm == null)
        {
            return BadRequest(new { message = "Dữ liệu giỏ hàng không hợp lệ." });
        }

        try
        {
            var updated = await _cartService.UpdateCartAsync(userId, bookId, cartVm);
            if (updated)
            {
                return Ok(new { message = "Cập nhật giỏ hàng thành công." });
            }
            return NotFound(new { message = $"Không tìm thấy giỏ hàng với UserId: {userId}, BookId: {bookId}." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Lỗi xử lý: {ex.Message}" });
        }
    }

    // Xóa một mục khỏi giỏ hàng
    [HttpDelete("user/{userId}/book/{bookId}")]
    public async Task<IActionResult> DeleteCart(int userId, int bookId)
    {
        try
        {
            var deleted = await _cartService.DeleteByIdAsync(userId, bookId);
            if (deleted)
            {
                return Ok(new { message = "Xóa giỏ hàng thành công." });
            }
            return NotFound(new { message = $"Không tìm thấy giỏ hàng với UserId: {userId}, BookId: {bookId}." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Lỗi xử lý: {ex.Message}" });
        }
    }
}
