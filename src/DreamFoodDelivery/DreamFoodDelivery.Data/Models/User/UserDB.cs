using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    class UserDB
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string EMail { get; set; }
        public string Role { get; set; }
        public UserInfoDB UserInfo { get; set; }
        public BasketDB Basket { get; set; }
        public HashSet<OrderDB> Orders { get; set; }
        public HashSet<CommentDB> Comments { get; set; }
    }
}
