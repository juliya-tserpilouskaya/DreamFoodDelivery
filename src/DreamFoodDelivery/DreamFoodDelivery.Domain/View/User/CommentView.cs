using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class CommentView
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        //public OrderView Order { get; set; }
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
        public DateTime? PostTime { get; set; }
        public DateTime? ModificationTime { get; set; }
    }
}
