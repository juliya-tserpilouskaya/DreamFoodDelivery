using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Orders
{
    public class Comment
    {
        //old
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string Headline { get; set; }
        public byte Rating { get; set; }
        public string Content { get; set; }
    }
}
