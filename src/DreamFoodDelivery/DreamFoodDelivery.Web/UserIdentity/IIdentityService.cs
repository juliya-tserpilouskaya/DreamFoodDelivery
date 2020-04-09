using DreamFoodDelivery.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Web
{
    public interface IIdentityService
    {
        Task<Result<string>> RegisterAsync(string email, string password);
    }
}
