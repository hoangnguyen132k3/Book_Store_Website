using BookStore.DAL.Data;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
    }

    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        private readonly BookDbContext _context;

        public OrderItemRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
