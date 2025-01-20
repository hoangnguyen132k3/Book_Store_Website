using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public interface IUserService : IBaseService<User>
    {
        Task<int> AddUserAsync(UserVm userVm);
        Task<bool> UpdateUserAsync(int id, UserVm userVm);

        Task<bool> DeleteUserByIdAsync(int id);
        Task<bool> DeleteUsersByIdAsync(List<int> userIds);

        Task<UserVm?> GetByUserIdAsync(int id);

        Task<PagedResult<UserVm>> GetAllUserAsync(int? pageNumber, int? pageSize);
        Task<User?> GetUserByEmailAsync(string email);
        Task<PagedResult<UserVm>> FilterUsersAsync(string? searchTerm, int? days, int? pageNumber, int? pageSize);
        Task<IdentityResult> ChangePasswordAsync(int userId, ChangePasswordVm model);
        Task<bool> ToggleBlockUserAsync(int userId);
        Task<bool> ToggleBlockUsersAsync(List<int> userIds);

    }
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDetailsService _userDetailsService;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, IUserDetailsService userDetailsService, UserManager<User> userManager) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userDetailsService = userDetailsService;
            _userManager = userManager;
        }

        public async Task<int> AddUserAsync(UserVm userVm)
        {
            var user = new User()
            {
                Email = userVm.Email,
                Status = userVm.Status,
                Role = userVm.Role,
                UserName = userVm.Email
            };

            var result = await _userManager.CreateAsync(user, userVm.PasswordHash);
            if (result.Succeeded)
            {
                await _unitOfWork.SaveChangesAsync();
                await _userDetailsService.AddUserDetailsAsync(user.UserId, userVm);
                return user.UserId;
            }
            var passwordErrorMessages = result.Errors
                .Where(e => e.Code.Contains("Password"))
                .Select(e => e.Description)
                .ToList();

            if (passwordErrorMessages.Any())
            {
                throw new ExceptionBusinessLogic("Mật khẩu phải bao gồm: 6 ký tự trở lên chứa chữ hoa, chữ thường và ký tự số");
            }
            throw new ExceptionBusinessLogic(string.Join(", ", result.Errors.Select(e => e.Description)));

        }


        public async Task<bool> UpdateUserAsync(int id, UserVm userVm)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new ExceptionNotFound("User không tìm thấy.");
            }
            user.Email = userVm.Email;
            user.Status = userVm.Status;
            user.Role = userVm.Role;
            user.LastOnlineAt = userVm.LastOnlineAt;
            if (!string.IsNullOrEmpty(userVm.PasswordHash))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    throw new ExceptionBusinessLogic("Lỗi trong quá trình đổi mật khẩu.");
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, userVm.PasswordHash);
                if (!addPasswordResult.Succeeded)
                {
                    var passwordErrorMessages = addPasswordResult.Errors
                        .Where(e => e.Code.Contains("Password"))
                        .Select(e => e.Description)
                        .ToList();

                    if (passwordErrorMessages.Any())
                    {
                        throw new ExceptionBusinessLogic("Mật khẩu phải bao gồm: 6 ký tự trở lên chứa chữ hoa, chữ thường và ký tự số.");
                    }
                    throw new ExceptionBusinessLogic(string.Join(", ", addPasswordResult.Errors.Select(e => e.Description)));
                }
            }

            await UpdateAsync(user);
            await _userDetailsService.UpdateUserDetailsAsync(id, userVm);
            _unitOfWork.Context.Entry(user).State = EntityState.Modified;
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await _userDetailsService.DeleteUserDetailsByUserIdAsync(id);
                await DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteUsersByIdAsync(List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("Danh sách UserId cần xóa không được để trống.", nameof(userIds));

            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var usersToDelete = users.Where(user => userIds.Contains(user.UserId)).ToList();

            if (!usersToDelete.Any())
                throw new ExceptionNotFound("Không tìm thấy người dùng nào với các UserId đã cung cấp.");

            await _unitOfWork.BeginTransactionAsync();  
            try
            {
                foreach (var user in usersToDelete)
                {
                    await _userDetailsService.DeleteUserDetailsByUserIdAsync(user.UserId);
                    await DeleteAsync(user.UserId);
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<UserVm?> GetByUserIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(
                filter: u => u.UserId == id,
                include: query => query.Include(u => u.UserDetails)
            );
            if (user == null)
            {
                return null;
            }
            var result = new UserVm
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails?.FullName,
                DateOfBirth = user.UserDetails?.DateOfBirth,
                Gender = user.UserDetails?.Gender,
                Address = user.UserDetails?.Address,
                PhoneNumber = user.UserDetails?.PhoneNumber
            };

            return result;
        }

        public async Task<PagedResult<UserVm>> GetAllUserAsync(int? pageNumber, int? pageSize)
        {
            int currentPage = pageNumber ?? 1; 
            int currentPageSize = pageSize ?? 10; 

            var query = await _unitOfWork.UserRepository.GetAllAsync(
                filter: null,
                include: q => q.Include(u => u.UserDetails)
            );

            var totalCount = query.Count();
            var paginatedUsers = query
                .OrderBy(u => u.UserId)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            var userVms = paginatedUsers.Select(user => new UserVm
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails?.FullName,
                DateOfBirth = user.UserDetails?.DateOfBirth,
                Gender = user.UserDetails?.Gender,
                Address = user.UserDetails?.Address,
                PhoneNumber = user.UserDetails?.PhoneNumber
            });

            return new PagedResult<UserVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = userVms
            };
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.UserRepository
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<IdentityResult> ChangePasswordAsync(int userId, ChangePasswordVm model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new KeyNotFoundException("User không tìm thấy.");
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

            if (!isCorrectPassword)
            {
                throw new UnauthorizedAccessException("Password hiện tại không chính xác.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            return result;
        }

        public async Task<PagedResult<UserVm>> FilterUsersAsync(string? searchTerm, int? days, int? pageNumber, int? pageSize)
        {
            int currentPage = pageNumber ?? 1; 
            int currentPageSize = pageSize ?? 10; 

            DateTime? cutoffDate = days.HasValue ? DateTime.Now.AddDays(-days.Value) : null;

            searchTerm = searchTerm?.Trim()?.ToLower();

            var query = await _unitOfWork.UserRepository.GetAllAsync(
                filter: user =>
                    (string.IsNullOrEmpty(searchTerm) || 
                    user.Email.ToLower().Contains(searchTerm) || 
                    user.UserDetails.FullName.ToLower().Contains(searchTerm)) 
                    && (!cutoffDate.HasValue || user.LastOnlineAt >= cutoffDate), 
                include: query => query.Include(u => u.UserDetails) 
            );

            var totalCount = query.Count();

            var paginatedUsers = query
                .OrderBy(user => user.UserId) 
                .Skip((currentPage - 1) * currentPageSize) 
                .Take(currentPageSize) 
                .ToList();
            var userVms = paginatedUsers.Select(user => new UserVm
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails.FullName,
                DateOfBirth = user.UserDetails.DateOfBirth,
                Gender = user.UserDetails.Gender,
                Address = user.UserDetails.Address,
                PhoneNumber = user.UserDetails.PhoneNumber
            });

            return new PagedResult<UserVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = userVms
            };
        }

        public async Task<bool> ToggleBlockUserAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User không tìm thấy.");
            }

            user.Status = !user.Status;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleBlockUsersAsync(List<int> userIds)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

                    if (user == null)
                    {
                        throw new ArgumentException("User không tìm thấy.");
                    }
                    user.Status = !user.Status;
                }
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("UserIds trống rỗng");
            }
        }
    }

}
