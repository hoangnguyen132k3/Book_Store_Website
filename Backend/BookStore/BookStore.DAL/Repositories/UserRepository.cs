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
    public interface IUserRepository : IGenericRepository<User>
    {
    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly BookDbContext _context;

        public UserRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
