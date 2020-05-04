using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamFoodDelivery.Domain.DTO
{
    public class UserEmailToChange
    {
        public string IdFromIdentity { get; set; }
        //public string Token { get; set; }
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
