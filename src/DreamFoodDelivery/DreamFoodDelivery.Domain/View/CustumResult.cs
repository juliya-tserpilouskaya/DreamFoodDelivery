using DreamFoodDelivery.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DreamFoodDelivery.Domain.View
{
    public class CustumResult
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
