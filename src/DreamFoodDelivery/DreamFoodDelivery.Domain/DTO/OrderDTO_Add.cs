using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderDTO_Add
    {
        public Guid UserId { get; set; }
        public Guid BasketId { get; set; }
        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public double? FinaleCost { get; set; }
        public double? ShippingСost { get; set; }
        public string Status { get; set; }
    }
}
