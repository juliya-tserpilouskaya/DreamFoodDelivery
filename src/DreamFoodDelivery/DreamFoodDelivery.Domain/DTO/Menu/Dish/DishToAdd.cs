using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class DishToAdd
    {
        public string Name { get; set; }
        //public int? Category { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double? Cost { get; set; }
        public string Weigh { get; set; }
        public int? Sale { get; set; }
        // public HashSet<TagToAdd> TagIndexes { get; set; }
        public HashSet<TagToAdd> TagNames { get; set; }
    }
}
