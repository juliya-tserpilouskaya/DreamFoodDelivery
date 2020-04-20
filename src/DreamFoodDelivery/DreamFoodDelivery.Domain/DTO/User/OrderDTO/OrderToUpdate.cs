using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderToUpdate
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public double? ShippingСost { get; set; }
    }
}
