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
    public interface IUserDetailsRepository : IGenericRepository<UserDetails>
    {
    }

    public class UserDetailsRepository : GenericRepository<UserDetails>, IUserDetailsRepository
    {
        private readonly BookDbContext _context;

        public UserDetailsRepository(BookDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
