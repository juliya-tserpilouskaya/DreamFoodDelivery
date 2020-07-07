using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class UserDB
    {
        public Guid Id { get; set; }
        public string IdFromIdentity { get; set; }
        public BasketDB Basket { get; set; }
        public Guid BasketId { get; set; }
        public HashSet<OrderDB> Orders { get; set; }
        public HashSet<ReviewDB> Reviews { get; set; }
        public HashSet<RefreshToken> RefreshTokens { get; set; }
    }
}
