﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderToStatusUpdate
    {
        public Guid Id { get; set; }
        public int? StatusIndex { get; set; }
    }
}
