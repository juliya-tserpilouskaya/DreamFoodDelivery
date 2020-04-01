using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class CommentDTO_View
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public Guid OrderId { get; set; }
        public OrderDTO_View Order { get; set; }
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
        public DateTime? PostTime { get; set; };
        public DateTime? ModificationTime { get; set; }
    }
}
