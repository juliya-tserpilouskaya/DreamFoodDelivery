using DreamFoodDelivery.Domain.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Baskets
{
    public class Pottle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<Dish> DishItems { get; set; }
    }
}
