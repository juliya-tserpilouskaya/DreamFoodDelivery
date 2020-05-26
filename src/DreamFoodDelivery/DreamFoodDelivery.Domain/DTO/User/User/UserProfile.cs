using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserProfile
    {
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public double PersonalDiscount { get; set; }
    }
}
