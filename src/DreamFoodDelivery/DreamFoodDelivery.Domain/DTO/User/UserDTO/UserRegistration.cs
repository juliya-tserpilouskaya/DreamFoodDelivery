using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserRegistration
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
    }
}
