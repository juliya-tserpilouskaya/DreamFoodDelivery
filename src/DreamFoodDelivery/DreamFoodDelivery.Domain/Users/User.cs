using DreamFoodDelivery.Domain.Baskets;
using DreamFoodDelivery.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Users
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public List<Pottle> Basket { get; set; }
        public List<Order> Orders { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
