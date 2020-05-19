using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class PageRequest
    {
        public PageRequest(int pageNumber = 1, int pageSize = 3)
        {
            PageNumber = (pageNumber >= 1 ? pageNumber : 1);
            PageSize = (pageSize >= 1 ? pageSize : 3);
        }

        public int PageNumber { get; }

        public int PageSize { get; }
    }

    public class PageRequest<T> : PageRequest
    {
        public PageRequest(T data, int pageNumber = 1, int pageSize = 3) : base(pageNumber, pageSize)
        {
            Data = data;
        }

        public T Data { get; }
    }
}
