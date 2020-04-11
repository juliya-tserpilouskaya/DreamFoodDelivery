using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain
{
    public class UserToUpdate
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public double PersonalDiscount { get; set; }
    }
}
