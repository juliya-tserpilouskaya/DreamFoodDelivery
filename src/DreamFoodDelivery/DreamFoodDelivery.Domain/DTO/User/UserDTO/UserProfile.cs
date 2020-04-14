using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserProfile
    {
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
