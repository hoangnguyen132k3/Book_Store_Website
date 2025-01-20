using BookStore.BLL.Services.ViewModels;
using BookStore.BLL.Services;
using BookStore.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserDetailsService _userDetailsService;

        public UsersController(IUserService userService, IUserDetailsService userDetailsService)
        {
            _userService = userService;
            _userDetailsService = userDetailsService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<UserVm>>> GetAllUsers(int? pageNumber, int? pageSize)
        {
            var users = await _userService.GetAllUserAsync(pageNumber, pageSize);
            if (users == null || !users.Items.Any())
            {
                return NotFound("Không tìm thấy người dùng nào.");
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserVm>> GetUserById(int id)
        {
            var user = await _userService.GetByUserIdAsync(id);
            if (user == null)
            {
                return NotFound($"User với ID {id} không tìm thấy.");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] UserVm user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu người dùng trống." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = ModelState });
                }

                var existingUser = await _userService.GetUserByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "Email đã tồn tại." });
                }

                var newUserId = await _userService.AddUserAsync(user);
                if (newUserId > 0)
                {
                    return Ok(new { success = true, message = "Thêm thành công." });
                }
                return BadRequest(new { success = false, message = "Có lỗi trong quá trình thêm người dùng." });
            }

            catch (ExceptionBusinessLogic ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi không mong muốn.", errorDetails = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserVm userVm)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("User ID không hợp lệ.");
                }

                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User không tìm thấy.");
                }
                else
                {
                    var result = await _userService.UpdateUserAsync(id, userVm);

                    if (result)
                    {
                        return Ok("Cập nhật thành công");
                    }
                    return BadRequest("Một số lỗi");
                }
            }
            catch (ExceptionBusinessLogic ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUserByIdAsync(id);
            if (!deletedUser)
            {
                return NotFound($"User với ID {id} không tìm thấy.");
            }

            return Ok("Xóa thành công");
        }
        [HttpDelete("delete-users-by-id-list")]
        public async Task<IActionResult> DeleteUsersByIdList([FromBody] List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return BadRequest("Danh sách UserId không hợp lệ.");
            }

            try
            {
                var result = await _userService.DeleteUsersByIdAsync(userIds);

                if (result)
                {
                    return Ok("Xóa người dùng thành công.");
                }
                else
                {
                    return BadRequest("Không tìm thấy người dùng để xóa.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User chưa được đăng nhập.");
                }

                var userId = int.Parse(userIdClaim.Value);

                var result = await _userService.ChangePasswordAsync(userId, model);

                if (result.Succeeded)
                {
                    return Ok("Đã thay đổi mật khẩu thành công.");
                }

                return BadRequest("Không thể thay đổi mật khẩu.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("filter-by-last-active/{days}")]
        public async Task<ActionResult<List<User>>> FilterUsersByLastActive(int days)
        {
            if (days < 0)
            {
                return BadRequest("Ngày phải là số nguyên không âm.");
            }

            var filteredUsers = await _userService.FilterUsersAsync(null, days, null, null);

            if (filteredUsers == null || !filteredUsers.Items.Any())
            {
                return NotFound($"Không tìm thấy user nào hoạt động lần cuối {days} ngày trước.");
            }

            return Ok(filteredUsers);
        }

        [HttpGet("filter-search/{query}")]
        public async Task<ActionResult<List<User>>> FilterUsersByKeySearch(string query)
        {
            query = query.ToLower().Trim();
            if (query == "")
            {
                return BadRequest("Tìm kiếm chính không thể trống.");
            }

            var filteredUsers = await _userService.FilterUsersAsync(query, null, null, null);

            if (filteredUsers == null || !filteredUsers.Items.Any())
            {
                return NotFound($"Không tìm thấy người dùng nào");
            }

            return Ok(filteredUsers);
        }

        [HttpPost("toggle-block/{id}")]
        public async Task<IActionResult> ToggleBlockUser(int id)
        {
            var result = await _userService.ToggleBlockUserAsync(id);

            if (result)
            {
                return Ok(new { success = true, message = "Cập nhật trạng thái người dùng thành công!" });
            }
            return BadRequest("Không tìm thấy người dùng nào với các UserId đã cung cấp.");
        }
        [HttpPost("toggle-block-users")]
        public async Task<IActionResult> ToggleBlockUsersAsync([FromBody] List<int> userIds)
        {
            var result = await _userService.ToggleBlockUsersAsync(userIds);

            if (result)
            {
                return Ok(new { success = true, message = "Cập nhật trạng thái các người dùng thành công!" });
            }
            return BadRequest("Không tìm thấy người dùng nào với các UserId đã cung cấp.");
        }
    }

}
