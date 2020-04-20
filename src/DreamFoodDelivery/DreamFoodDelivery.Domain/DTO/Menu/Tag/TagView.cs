using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class TagView
    {
        public Guid Id { get; set; }
        public HashSet<DishTagView> Dishes { get; set; }
        public int? IndexNumber { get; set; } 
    }
}
