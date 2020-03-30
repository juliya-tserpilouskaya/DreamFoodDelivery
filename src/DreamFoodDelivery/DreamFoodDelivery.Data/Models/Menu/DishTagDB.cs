using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class DishTagDB
    {
        public Guid TagId { get; set; }
        public Guid DishId { get; set; }
        public TagDB Tag { get; set; }
        public DishDB Dish { get; set; }
    }
}
