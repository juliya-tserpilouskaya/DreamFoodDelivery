﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Models
{
    public class DishDB
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public string Weight { get; set; }
        public double? Sale { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Modified { get; set; }
        public HashSet<DishTagDB> DishTags { get; set; }
        public DishDB()
        {
            DishTags = new HashSet<DishTagDB>();
        }
    }
}
