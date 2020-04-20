using DreamFoodDelivery.Common;
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
        Task<Result<IEnumerable<UserView>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        Task<Result<UserDTO>> CreateAccountAsyncById(string userIdFromIdentity);

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<UserViewSecondPlan>> GetByIdAsync(string userId);

        /// <summary>
        /// Get User idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result<UserDTO>> GetUserByIdFromIdentityAsync(string idFromIdentity);

        /// <summary>
        ///  Asynchronously update user
        /// </summary>
        /// <param name="user">Existing user to update</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserProfile>> UpdateUserProfileAsync(UserProfile user, string idFromIdentity);

        /// <summary>
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserProfile>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity);

        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        Task<Result> RemoveByIdAsync(string userId);

        /// <summary>
        /// Remove User idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result> DeleteUserByIdFromIdentityAsync(string idFromIdentity);

        /// <summary>
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="id"></param>
        Task<Result> ChangeRoleAsync(string id);
    }
}
