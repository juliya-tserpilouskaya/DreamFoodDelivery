using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class ReviewDB
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserDB User { get; set; }
        public Guid OrderId { get; set; }
        public OrderDB Order { get; set; }
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
        public DateTime? PostTime { get; set; } = DateTime.Now;
        public DateTime? ModificationTime { get; set; }
    }
}
