using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class BasketDTO
    {
        public Guid Id { get; set; }
        public HashSet<DishDTO> Dishes { get; set; }
        public DateTime? Modified { get; set; }
        public Guid UserId { get; set; }
    }
}
