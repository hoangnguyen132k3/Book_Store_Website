using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class IOrderService : BaseService<Order>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Order> _orderRepository;

        public IOrderService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = unitOfWork.GenericRepository<Order>();
        }

        public async Task<int> AddAsync(Order entity)
        {
            await _orderRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentVm paymentVm)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                if (paymentVm.PaymentMethod == "Credit Card")
                {

                }
                else if (paymentVm.PaymentMethod == "PayPal")
                {

                }

                order.PaymentStatus = "Paid"; 
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                if (order.PaymentStatus == "Paid")
                {
                    order.OrderStatus = "Completed";  
                    await _orderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> UpdateAsync(Order entity)
        {
            await _orderRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _orderRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _orderRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }
    }
}
