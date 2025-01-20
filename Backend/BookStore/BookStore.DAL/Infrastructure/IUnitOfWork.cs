using BookStore.DAL.Data;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BookStore.DAL.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        BookDbContext Context { get; }

        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<UserDetails> UserDetailsRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Book> BookRepository { get; }
        IGenericRepository<Cart> CartRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderItem> OrderItemRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<InformationType> SpecificationTypeRepository { get; }
        IGenericRepository<BookInformation> BookSpecificationRepository { get; }

        IGenericRepository<TEntity> GenericRepository<TEntity>() where TEntity : class;

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
