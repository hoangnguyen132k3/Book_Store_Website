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
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }

    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly BookDbContext _context;

        public OrderRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
