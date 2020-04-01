using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserInfoDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserDB User { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double PersonalDiscount { get; set; }
    }
}
