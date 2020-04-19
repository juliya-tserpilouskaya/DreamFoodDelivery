using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderToAdd
    {
        public Guid BasketId { get; set; }

        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }

        public double? OrderСost { get; set; }
        public double? ShippingСost { get; set; } 
    }
}
