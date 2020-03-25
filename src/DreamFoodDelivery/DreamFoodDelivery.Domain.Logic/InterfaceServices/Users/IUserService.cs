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

        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        Task<Result<User>> CreateAccountAsync(User user);

        /// <summary>
        /// Get user account by Id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        Task<User> GetByIdAsync(string id);

        /// <summary>
        /// Update the user account
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        Task<Result<User>> UpdateAsync(User user);

        /// <summary>
        /// Remove user by Id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);
    }
}
