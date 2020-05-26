using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class PasswordRecoveryRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string CallBackUrl { get; set; }
    }
}
