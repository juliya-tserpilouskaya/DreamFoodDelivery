using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderView
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BasketId { get; set; }
        public Guid CommentId { get; set; }
        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public double? FinaleCost { get; set; }
        public double? ShippingСost { get; set; }
        public string Status { get; set; } //enum?
        public bool Paid { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public DateTime? PaymentTime { get; set; }
    }
}
