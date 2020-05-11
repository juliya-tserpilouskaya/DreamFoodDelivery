using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.View
{
    public class UserWithToken
    {
        public string UserToken { get; set; }
        public UserView UserView { get; set; }
    }
}
