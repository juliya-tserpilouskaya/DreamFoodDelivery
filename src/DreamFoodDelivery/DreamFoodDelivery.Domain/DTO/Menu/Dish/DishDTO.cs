using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class DishDTO
    {
        //Remove it?
        public string Name { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Weight { get; set; }
        public double Sale { get; set; }
    }
}
