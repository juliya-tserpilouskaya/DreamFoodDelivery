using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class CommentToUpdate
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
    }
}
