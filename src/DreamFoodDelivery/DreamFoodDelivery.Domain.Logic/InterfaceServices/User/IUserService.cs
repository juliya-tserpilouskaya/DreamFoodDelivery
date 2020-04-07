using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IUserService
    {
        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        Task<Result<IEnumerable<UserDTO>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="user">New user to add</param>
        Task<Result<UserDTO>> CreateAccountAsync(UserDTO user);

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<UserDTO>> GetByIdAsync(string id);

        /// <summary>
        ///  Asynchronously update user
        /// </summary>
        /// <param name="user">Existing user to update</param>
        Task<Result<UserDTO>> UpdateAsync(UserDTO user);

        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        Task<Result> RemoveByIdAsync(string userId);
    }
}
