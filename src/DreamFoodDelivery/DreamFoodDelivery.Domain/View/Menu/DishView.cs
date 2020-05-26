using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class DishView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string Weigh { get; set; }
        public double Sale { get; set; }
        public double FinaleCost { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Modified { get; set; }
        public HashSet<TagToAdd> TagList { get; set; }
        public int Quantity { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
