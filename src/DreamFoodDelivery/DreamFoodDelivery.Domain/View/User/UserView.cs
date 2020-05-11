using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class UserView
    {
        public UserDTO UserDTO { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
