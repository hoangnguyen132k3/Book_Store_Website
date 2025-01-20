using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.InterfaceServices;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class CartService : BaseService<Cart>, ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Cart> _repository;

        public CartService(IUnitOfWork unitOfWork, IGenericRepository<Cart> repository) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<int> AddCartAsync(CartVm cartVm)
        {
            if (cartVm == null)
            {
                throw new ArgumentNullException(nameof(cartVm), "Dữ liệu giỏ hàng không hợp lệ.");
            }

            var existingCart = await _repository.GetFirstOrDefaultAsync(c =>
                c.UserId == cartVm.UserId && c.BookId == cartVm.BookId);

            if (existingCart != null)
            {
                // Nếu đã tồn tại, cập nhật số lượng và thời gian
                existingCart.Quantity += cartVm.Quantity;
                existingCart.AddedAt = DateTime.UtcNow;
                _repository.Update(existingCart);
            }
            else
            {
                // Nếu chưa tồn tại, thêm mới
                var newCart = new Cart
                {
                    UserId = cartVm.UserId,
                    BookId = cartVm.BookId,
                    Quantity = cartVm.Quantity,
                    AddedAt = DateTime.UtcNow
                };
                await _repository.AddAsync(newCart);
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateCartAsync(int userId, int bookId, CartVm cartVm)
        {
            var cart = await _repository.GetByCompositeKeyAsync(new object[] { userId, bookId });
            if (cart == null)
            {
                return false;
            }

            cart.Quantity = cartVm.Quantity;
            cart.AddedAt = cartVm.AddedAt != default ? cartVm.AddedAt : DateTime.UtcNow;
            _repository.Update(cart);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteByIdAsync(int userId, int bookId)
        {
            var cart = await _repository.GetByCompositeKeyAsync(new object[] { userId, bookId });
            if (cart == null)
            {
                return false;
            }

            _repository.Delete(cart);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Cart> GetByCartIdAsync(int userId, int bookId)
        {
            return await _repository.GetByCompositeKeyAsync(new object[] { userId, bookId });
        }

        public async Task<IEnumerable<Cart>> GetAllCartAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Thêm phương thức GetCartsByUserIdAsync
        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId)
        {
            var allCarts = await _repository.GetAllAsync();
            return allCarts.Where(cart => cart.UserId == userId);
        }


    }
}
