using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookStore.DAL.Data;

namespace BookStore.DAL.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BookDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(BookDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task<T> GetByIdAsync(int id)
        {
            // Lấy đối tượng dựa trên khóa chính
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            return await _context.Set<T>().FindAsync(keyValues);
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }
        public async Task<int> ModifyAsync(T entity)
        {
            _dbSet.Update(entity);

            int affectedRows = await _context.SaveChangesAsync();

            return affectedRows;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Guid id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public void Delete(T entity)
        {

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includesProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var property in includesProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query.Include(property);
                }
            }


            return orderBy != null ? orderBy(query) : query;
        }
        public async Task<T?> GetAsync(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
)
        {
            IQueryable<T> query = _dbSet;

            query = query.Where(filter);

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.FirstOrDefaultAsync();
        }


        public T? GetById(Guid id)
        {
            return _dbSet.Find(id);
        }
        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public IQueryable<T> GetQuery()
        {
            return _dbSet;
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return query;
        }
        public async Task<T?> GetByCompositeKeyAsync(object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 2)
            {
                throw new ArgumentException("Phải cung cấp đúng 2 giá trị cho Composite Key.");
            }

            return await _dbSet.FindAsync(keyValues);
        }

    }
}
