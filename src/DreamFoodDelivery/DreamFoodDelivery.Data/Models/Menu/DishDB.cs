using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class DishDB
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        // public int? Category { get; set; } 
        public string Composition { get; set; }
        public string Description { get; set; }
        public double? Cost { get; set; }
        public string Weigh { get; set; }
        public double? Sale { get; set; }
        //public Guid TagId { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Modified { get; set; }
        public HashSet<DishTagDB> DishTags { get; set; }
        public DishDB()
        {
            DishTags = new HashSet<DishTagDB>();
        }
    }
}
