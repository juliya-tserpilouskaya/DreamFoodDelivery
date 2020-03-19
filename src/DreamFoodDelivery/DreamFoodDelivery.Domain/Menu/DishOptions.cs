using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Menu
{
    public class DishOptions
    {
        //Database table optimization experiment
        public string DishId { get; set; } = Guid.NewGuid().ToString();
        public bool New { get; set; }
        public bool Pop { get; set; }
        public bool Hot { get; set; }
        //public int Sale { get; set; }
    }
}
