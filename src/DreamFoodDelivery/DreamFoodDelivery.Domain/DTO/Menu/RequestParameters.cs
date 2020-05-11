using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class RequestParameters
    {
        public string Request { get; set; }
        //public IEnumerable<int?> TagsIndexNumbers { get; set; }
        public IEnumerable<string?> TagsNames { get; set; }
        public bool OnSale { get; set; }
        public double LowerPrice { get; set; }
        public double UpperPrice { get; set; }
    }
}
