using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class BasketDB
    {
        public Guid Id { get; set; }
        public HashSet<DishDB> Dishes { get; set; }
        public DateTime? ModificationTime { get; set; }
        public Guid UserId { get; set; }
        public UserDB User { get; set; }
    }
}
