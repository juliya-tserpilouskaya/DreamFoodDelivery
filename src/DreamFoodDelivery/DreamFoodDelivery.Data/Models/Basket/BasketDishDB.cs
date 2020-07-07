using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class BasketDishDB
    {
        public Guid Id { get; set; }
        public Guid? BasketId { get; set; }
        public Guid? DishId { get; set; }
        public double? DishPrice { get; set; }
        public double? Sale { get; set; }
        public int? Quantity { get; set; }
        public Guid? OrderId { get; set; }
    }
}
