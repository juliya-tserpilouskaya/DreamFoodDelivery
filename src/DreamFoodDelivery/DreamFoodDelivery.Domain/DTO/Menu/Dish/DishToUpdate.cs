﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class DishToUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string Weigh { get; set; }
        public double Sale { get; set; }
        public HashSet<TagToAdd> TagNames { get; set; }
    }
}
