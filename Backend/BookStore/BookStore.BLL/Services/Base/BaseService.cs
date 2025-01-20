using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddAsync(T entity)
        {
            await _unitOfWork.GenericRepository<T>().AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            await _unitOfWork.GenericRepository<T>().UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            await _unitOfWork.GenericRepository<T>().DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(int id, int bookId)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _unitOfWork.GenericRepository<T>().GetAllAsync();
        }

        public virtual int Add(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Add(entity);
                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(entity));
        }

        public virtual int Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                _unitOfWork.GenericRepository<T>().Delete(id);
                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(id));
        }

        public virtual int Delete(int id)
        {
            try
            {
                _unitOfWork.GenericRepository<T>().Delete(id);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(nameof(id));
            }
        }

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                _unitOfWork.GenericRepository<T>().Delete(id);
                return await _unitOfWork.SaveChangesAsync();
            }

            throw new ArgumentNullException(nameof(id));
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Delete(entity);
                var changes = await _unitOfWork.SaveChangesAsync();
                return changes > 0;
            }

            throw new ArgumentNullException(nameof(entity), "Thực thể cần xóa không thể rỗng.");
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _unitOfWork.GenericRepository<T>().GetAll();
        }

        public virtual T? GetById(Guid id)
        {
            return _unitOfWork.GenericRepository<T>().GetById(id);
        }

        public virtual T? GetById(int id)
        {
            return _unitOfWork.GenericRepository<T>().GetById(id);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }


        public virtual int Update(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Update(entity);
                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(entity));
        }

        public async Task<PaginatedResult<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = "",
            int pageIndex = 1,
            int pageSize = 10)
        {
            var repository = _unitOfWork.GenericRepository<T>();
            IQueryable<T> query = repository.GetQuery();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await PaginatedResult<T>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = "")
        {
            var repository = _unitOfWork.GenericRepository<T>();
            IQueryable<T> query = repository.GetQuery();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = "")
        {
            var repository = _unitOfWork.GenericRepository<T>();
            IQueryable<T> query = repository.GetQuery();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void ValidateModelPropertiesWithAttribute<T>(T model)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(typeof(SkipValidationAttribute), false).Any())
                {
                    continue;
                }

                var value = property.GetValue(model);

                if (value == null)
                {
                    throw new ArgumentException($"Property '{property.Name}' in model '{typeof(T).Name}' cannot be null.");
                }

                if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
                {
                    throw new ArgumentException($"Property '{property.Name}' in model '{typeof(T).Name}' cannot be an empty string or whitespace.");
                }
            }
        }

        public async Task<(int countRemoved, int countUpdated)> DeleteMultipleAsync(
            IEnumerable<int> ids,
            Func<T, bool> condition,
            Func<T?, Task> onConditionFailed)
        {
            await _unitOfWork.BeginTransactionAsync();

            int countRemoved = 0;
            int countUpdated = 0;

            try
            {
                foreach (var id in ids)
                {
                    var entity = await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
                    if (entity == null || id == 0)
                    {
                        throw new ExceptionBusinessLogic($"'{typeof(T).Name}' not found.");
                    }
                    if (condition(entity))
                    {
                        await _unitOfWork.GenericRepository<T>().DeleteAsync(id);
                        countRemoved++;
                    }
                    else
                    {
                        await onConditionFailed(entity);
                        countUpdated++;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (countRemoved, countUpdated);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                throw new ArgumentException("Failed to delete entities. " + ex.Message, ex);
            }
        }

        public async Task<int> RestoreMultipleAsync(
            IEnumerable<int> ids,
            Func<T, bool> condition,
            Func<T?, Task> onCondition)
        {
            await _unitOfWork.BeginTransactionAsync();

            int countUpdated = 0;

            try
            {
                foreach (var id in ids)
                {
                    var entity = await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
                    if (entity == null)
                    {
                        throw new ExceptionBusinessLogic($"'{typeof(T).Name}' not found.");
                    }
                    if (condition(entity))
                    {
                        await onCondition(entity);
                        countUpdated++;
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return countUpdated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                throw new ArgumentException("Failed to delete entities. " + ex.Message, ex);
            }

        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }



    }
}
