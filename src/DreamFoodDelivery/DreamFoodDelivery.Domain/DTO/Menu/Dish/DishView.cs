using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class DishView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Category { get; set; }
        public string Сomposition { get; set; }
        public string Description { get; set; }
        public double? Cost { get; set; }
        public string Weigh { get; set; }
        public int? Sale { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Modified { get; set; }
        public HashSet<TagToAdd> TagList { get; set; }
        public int? Quantity { get; set; }
    }
}
