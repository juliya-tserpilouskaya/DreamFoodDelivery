using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class TagDB
    {
        public Guid Id { get; set; }
        public string TagName { get; set; }
        public HashSet<DishTagDB> DishTags { get; set; }
        public TagDB()
        {
            DishTags = new HashSet<DishTagDB>();
        }
    }
}
