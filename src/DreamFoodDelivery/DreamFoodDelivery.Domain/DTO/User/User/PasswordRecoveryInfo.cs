using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class PasswordRecoveryInfo
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string CallBackUrl { get; set; }

        [Required]
        public string Token { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
