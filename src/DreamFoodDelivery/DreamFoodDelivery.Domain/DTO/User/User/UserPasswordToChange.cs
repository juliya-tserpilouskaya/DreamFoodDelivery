using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserPasswordToChange
    {
        public string IdFromIdentity { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
