using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string IdFromIdentity { get; set; }
        public Guid BasketId { get; set; }
    }
}
