using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserUpdate
    {
        public Guid Id { get; set; }
        public string IdFromIdentity { get; set; }
        public Guid BasketId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public double PersonalDiscount { get; set; }
    }
}
