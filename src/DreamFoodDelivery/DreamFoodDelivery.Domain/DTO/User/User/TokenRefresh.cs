﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class TokenRefresh
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
