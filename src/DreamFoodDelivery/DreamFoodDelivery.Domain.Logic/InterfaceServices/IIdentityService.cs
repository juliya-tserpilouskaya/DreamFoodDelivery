using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IIdentityService
    {
        Task<Result<UserDTO>> RegisterAsync(string email, string password);
        Task<Result<string>> LoginAsync(string email, string password);
        Task<Result> DeleteAsync(string email, string password);
    }
}
