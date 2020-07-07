using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public UserDB User { get; set; }
        public DateTime Expires { get; set; }
        public bool Active => DateTime.UtcNow <= Expires;

        public RefreshToken(string token, DateTime expires, Guid userId)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
        }
    }
}
