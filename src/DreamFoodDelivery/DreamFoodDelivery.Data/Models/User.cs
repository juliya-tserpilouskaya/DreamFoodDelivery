using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class User : IdentityUser
    {
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
