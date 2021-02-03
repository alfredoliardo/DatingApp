using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(ICollection<T> items,int currentPage, int pageSize, int totalItems)
        {
            CurrentPage = currentPage;
            TotalPages = (int) Math.Ceiling(totalItems/(double)pageSize);
            PageSize = pageSize;
            TotalItems = totalItems;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public async static Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,int pageSize)
        {
            int count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}