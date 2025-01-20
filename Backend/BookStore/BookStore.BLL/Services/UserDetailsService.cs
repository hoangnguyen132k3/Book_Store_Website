using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public interface IUserDetailsService : IBaseService<UserDetails>
    {
        Task<int> AddUserDetailsAsync(int id, UserVm userVm);
        Task<bool> UpdateUserDetailsAsync(int userId, UserVm userVm);

        Task<bool> DeleteUserDetailsByUserIdAsync(int userId);
        Task<UserDetails?> GetByUserIdAsync(int id);
    }
    public class UserDetailsService : BaseService<UserDetails>, IUserDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserDetailsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddUserDetailsAsync(int id, UserVm userVm)
        {
            var details = new UserDetails()
            {
                UserId = id,
                FullName = userVm.FullName,
                DateOfBirth = userVm.DateOfBirth,
                Gender = userVm.Gender,
                Address = userVm.Address,
                PhoneNumber = userVm.PhoneNumber

            };
            await AddAsync(details);
            await _unitOfWork.SaveChangesAsync();
            return details.UserDetailsId;
        }

        public async Task<bool> UpdateUserDetailsAsync(int userId, UserVm userVm)
        {
            var detailsExists = await GetByUserIdAsync(userId);
            if (detailsExists == null)
            {
                var newUserDetailsId = await AddUserDetailsAsync(userId, userVm);
                return newUserDetailsId > 0;
            }

            detailsExists.FullName = userVm.FullName;
            detailsExists.DateOfBirth = userVm.DateOfBirth;
            detailsExists.Gender = userVm.Gender;
            detailsExists.Address = userVm.Address;
            detailsExists.PhoneNumber = userVm.PhoneNumber;

            await UpdateAsync(detailsExists);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<UserDetails?> GetByUserIdAsync(int userId)
        {
            return await _unitOfWork.UserDetailsRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> DeleteUserDetailsByUserIdAsync(int userId)
        {
            var details = await GetByUserIdAsync(userId);
            if (details != null)
            {
                await DeleteAsync(details.UserDetailsId);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
