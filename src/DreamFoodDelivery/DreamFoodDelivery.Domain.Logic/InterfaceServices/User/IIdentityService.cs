using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IIdentityService
    {
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">User registration data</param>
        Task<Result<UserWithToken>> RegisterAsync(UserRegistration user);

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        Task<Result<UserWithToken>> LoginAsync(string email, string password);

        /// <summary>
        /// Remove user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        Task<Result> DeleteAsync(string email, string password);
    }
}
