﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class OrderDB
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public UserDB User { get; set; }
        public Guid? BasketId { get; set; }
        public ReviewDB Review { get; set; }
        public Guid? ReviewId { get; set; }

        public bool IsInfoFromProfile { get; set; }
        public string Address { get; set; }
        public double? PersonalDiscount { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public double? TotalCost { get; set; }
        public double? DeliveryCost { get; set; }
        public string Status { get; set; } 
        public DateTime? OrderTime { get; set; } = DateTime.Now;
        public DateTime? DeliveryTime { get; set; }
        public DateTime? PaymentTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
