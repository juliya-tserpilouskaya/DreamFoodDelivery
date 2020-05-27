using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class OrderView
    {
        public Guid Id { get; set; }
        public HashSet<DishView> Dishes { get; set; }
        public Guid? CommentId { get; set; }

        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public double? TotalCost { get; set; }
        public double? ShippingCost { get; set; }
        public string Status { get; set; } 
        public DateTime? OrderTime { get; set; } 
        public DateTime? DeliveryTime { get; set; }
        public DateTime? PaymentTime { get; set; }
    }
}
