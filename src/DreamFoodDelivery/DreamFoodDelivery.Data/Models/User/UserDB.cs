using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class UserDB
    {
        public Guid Id { get; set; }
        public string IdFromIdentity { get; set; }
        //public string Login { get; set; }
        //public string Password { get; set; }
        //public string EMail { get; set; }
        //public string Role { get; set; }
        //public UserInfoDB UserInfo { get; set; }
        //public Guid UserInfoId { get; set; }
        public BasketDB Basket { get; set; }
        public Guid BasketId { get; set; }
        public HashSet<OrderDB> Orders { get; set; }
        public HashSet<CommentDB> Comments { get; set; }
    }
}
