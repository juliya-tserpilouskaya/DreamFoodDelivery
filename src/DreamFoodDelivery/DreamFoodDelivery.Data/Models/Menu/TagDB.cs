using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class TagDB
    {
        public Guid Id { get; set; }
        //public Guid DishId { get; set; }
        //public int? IndexNumber { get; set; } //Is it necessary? I can use a guid
        public string TagName { get; set; }
        public HashSet<DishTagDB> DishTags { get; set; }
        public TagDB()
        {
            DishTags = new HashSet<DishTagDB>();
        }
    }
}
