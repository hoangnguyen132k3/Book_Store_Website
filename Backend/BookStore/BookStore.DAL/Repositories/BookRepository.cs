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
    public interface IBookRepository : IGenericRepository<Book>
    {
    }

    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly BookDbContext _context;

        public BookRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
