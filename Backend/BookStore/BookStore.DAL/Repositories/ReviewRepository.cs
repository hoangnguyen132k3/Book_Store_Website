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
    public interface IReviewRepository : IGenericRepository<Review>
    {
    }

    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly BookDbContext _context;

        public ReviewRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
