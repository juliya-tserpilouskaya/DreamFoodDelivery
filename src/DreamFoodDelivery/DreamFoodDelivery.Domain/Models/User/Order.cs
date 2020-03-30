using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Models
{
    public class Order
    {
        //old
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string BasketId { get; set; }
        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public double FinaleCost { get; set; }
        public string ShippingСost { get; set; }
        public string Status { get; set; } //enum?
        public string Paid { get; set; }
    }
}
