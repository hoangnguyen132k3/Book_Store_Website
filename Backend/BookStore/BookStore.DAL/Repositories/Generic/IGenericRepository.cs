﻿using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(params object[] keyValues);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<int> ModifyAsync(T entity);
        Task DeleteAsync(int id);
        Task<T?> GetByCompositeKeyAsync(object[] keyValues);
        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);

        T? GetById(Guid id);
        Task<T?> GetByIdAsync(Guid id);
        T? GetById(int id);

        void Add(T entity);
        void Update(T entity);

        void Delete(Guid id);
        void Delete(int id);
        void Delete(T entity);

        IQueryable<T> GetQuery();
        IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate);
        IQueryable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = "");
        Task<T?> GetAsync(
                Expression<Func<T, bool>> filter,
                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
            );
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        );
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}
