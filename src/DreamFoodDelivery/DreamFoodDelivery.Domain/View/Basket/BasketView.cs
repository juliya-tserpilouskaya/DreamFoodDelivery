using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class BasketView
    {
        public Guid Id { get; set; }
        public HashSet<DishView> Dishes { get; set; }
        public DateTime? ModificationTime { get; set; }
        public double BasketCost { get; set; }
        public double? ShippingCost { get; set; }
    }
}
