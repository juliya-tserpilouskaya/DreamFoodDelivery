using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string EMail { get; set; }
        public string Role { get; set; }
        public UserInfoDTO UserInfo { get; set; }
        public Guid UserInfoId { get; set; }
        public BasketDTO Basket { get; set; }
        public Guid BasketId { get; set; }
        public HashSet<OrderView> Orders { get; set; }
        public HashSet<CommentView> Comments { get; set; }
    }
}
