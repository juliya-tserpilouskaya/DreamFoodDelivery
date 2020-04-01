using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class TagDTO
    {
        public Guid Id { get; set; }
        public HashSet<DishDTO> Dishes { get; set; }
        public int? IndexNumber { get; set; } //Is it necessary? I can use a guid
    }
}
