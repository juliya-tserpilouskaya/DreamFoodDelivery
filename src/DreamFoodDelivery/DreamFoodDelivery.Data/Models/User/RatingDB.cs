using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class RatingDB
    {
        public Guid Id { get; } = new Guid();
        public int? Count { get; set; }
        public int? Sum { get; set; }
    }
}
