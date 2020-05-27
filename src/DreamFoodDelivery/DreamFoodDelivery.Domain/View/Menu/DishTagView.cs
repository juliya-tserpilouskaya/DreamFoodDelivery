using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class DishTagView
    {
        public Guid TagId { get; set; }
        public Guid DishId { get; set; }
        public TagView Tag { get; set; }
        public DishView Dish { get; set; }
    }
}
