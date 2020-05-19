using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class PageResponse<T> where T : class
    {
        public PageResponse(IEnumerable<T> data, int count, int pageNumber, int pageSize)
        {
            Data = data;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public IEnumerable<T> Data { get; }

        public int PageNumber { get; }
        public int TotalPages { get; }


        public bool HasNextPage => (PageNumber < TotalPages);

        public bool HasPreviousPage => (PageNumber > 1);
    }
}
