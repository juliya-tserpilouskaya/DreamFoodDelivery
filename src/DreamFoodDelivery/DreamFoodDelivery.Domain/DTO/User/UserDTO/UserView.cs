using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserView
    {
        public Guid Id { get; set; }
        public string IdFromIdentity { get; set; }
        public Guid BasketId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double PersonalDiscount { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserDTO UserDTO { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
