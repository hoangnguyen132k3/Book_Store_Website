using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.InterfaceServices
{
    public interface ICartService : IBaseService<Cart>
    {
        Task<int> AddCartAsync(CartVm cartVm);

        Task<bool> UpdateCartAsync(int userId, int bookId, CartVm cartVm);

        Task<bool> DeleteByIdAsync(int userId, int bookId);

        Task<Cart?> GetByCartIdAsync(int userId, int bookId);

        Task<IEnumerable<Cart>> GetAllCartAsync();

        
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId);
    }
}
