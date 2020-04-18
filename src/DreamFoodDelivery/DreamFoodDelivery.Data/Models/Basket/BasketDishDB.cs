using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class BasketDishDB
    {
        public Guid BasketId { get; set; }
        public Guid ConnectionId { get; set; }
        public Guid DishId { get; set; }
        public int? Quantity { get; set; }
    }
}
