using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class ReviewForUsersView
    {
        public string Headline { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
