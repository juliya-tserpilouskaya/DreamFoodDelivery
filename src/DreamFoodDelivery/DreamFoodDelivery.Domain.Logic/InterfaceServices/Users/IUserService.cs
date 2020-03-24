using DreamFoodDelivery.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetAllAsync();
    }
}
