using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class ReviewToAdd
    {
        //public Guid UserId { get; set; }
        public string OrderId { get; set; }
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
    }
}
