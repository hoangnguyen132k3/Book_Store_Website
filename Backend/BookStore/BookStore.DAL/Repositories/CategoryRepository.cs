using BookStore.DAL.Data;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BookStore.DAL.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
    }

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly BookDbContext _context;

        public CategoryRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
