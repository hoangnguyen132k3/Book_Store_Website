using BookStore.DAL.Data;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _context;
        private IDbContextTransaction _currentTransaction;

        private IGenericRepository<User>? _userRepository;
        private IGenericRepository<UserDetails>? _userDetailsRepository;
        private IGenericRepository<Category>? _categoryRepository;
        private IGenericRepository<Book>? _bookRepository;
        private IGenericRepository<Cart>? _cartRepository;
        private IGenericRepository<Order>? _orderRepository;
        private IGenericRepository<OrderItem>? _orderItemRepository;
        private IGenericRepository<Review>? _reviewRepository;
        private IGenericRepository<InformationType>? _specificationTypeRepository;
        private IGenericRepository<BookInformation>? _bookSpecificationRepository;

        public UnitOfWork(BookDbContext context)
        {
            _context = context;
        }

        public BookDbContext Context => _context;

        public IGenericRepository<User> UserRepository =>
            _userRepository ??= new GenericRepository<User>(_context);

        public IGenericRepository<UserDetails> UserDetailsRepository =>
            _userDetailsRepository ??= new GenericRepository<UserDetails>(_context);

        public IGenericRepository<Category> CategoryRepository =>
            _categoryRepository ??= new GenericRepository<Category>(_context);

        public IGenericRepository<Book> BookRepository =>
            _bookRepository ??= new GenericRepository<Book>(_context);

        public IGenericRepository<Cart> CartRepository =>
            _cartRepository ??= new GenericRepository<Cart>(_context);

        public IGenericRepository<Order> OrderRepository =>
            _orderRepository ??= new GenericRepository<Order>(_context);

        public IGenericRepository<OrderItem> OrderItemRepository =>
            _orderItemRepository ??= new GenericRepository<OrderItem>(_context);

        public IGenericRepository<Review> ReviewRepository =>
            _reviewRepository ??= new GenericRepository<Review>(_context);

        public IGenericRepository<InformationType> SpecificationTypeRepository =>
            _specificationTypeRepository ??= new GenericRepository<InformationType>(_context);

        public IGenericRepository<BookInformation> BookSpecificationRepository =>
            _bookSpecificationRepository ??= new GenericRepository<BookInformation>(_context);

        public IGenericRepository<TEntity> GenericRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity>(_context);
        }

        // Xoá phương thức trùng lặp
        // public async Task<int> SaveChangesAsync() { ... }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task CommitAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}
