using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Image
{
    public class ImageModel
    {
        public IFormFile Image { get; set; }
        public string DishId { get; set; }
    }
}
