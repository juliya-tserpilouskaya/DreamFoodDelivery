using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class OrderDTO_Update_Status
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
