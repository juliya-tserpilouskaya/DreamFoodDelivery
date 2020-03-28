using DreamFoodDelivery.Domain.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Basket
{
    public class BasketModel
    {
        //old
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<Dish> DishItems { get; set; }
    }
}
