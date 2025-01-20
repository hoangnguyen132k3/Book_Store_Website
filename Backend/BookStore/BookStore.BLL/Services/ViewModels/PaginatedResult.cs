using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class PaginatedResult<T>
    {
        public int PageIndex { get; private set; } 
        public int TotalPages { get; private set; } 
        public T[] Items { get; private set; } = Array.Empty<T>(); 

        // Constructor
        public PaginatedResult(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); 
            Items = items.ToArray();
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
        {
            var count = await query.CountAsync(); 
            var items = await query.Skip((pageIndex - 1) * pageSize) 
                                    .Take(pageSize) 
                                    .ToListAsync(); 
            return new PaginatedResult<T>(items, count, pageIndex, pageSize);
        }
    }
}
