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
        Task<Result<UserView>> CreateAccountAsyncById(string userIdFromIdentity);

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<UserView>> GetByIdAsync(string userId);

        /// <summary>
        /// Get User Profile by idFromIdentity. Helper
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result<UserProfile>> GetUserProfileByIdFromIdentityAsync(string idFromIdentity);

        /// <summary>
        /// Get User idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result<UserView>> GetUserByIdFromIdentityAsync(string idFromIdentity);

        /// <summary>
        ///  Asynchronously update user
        /// </summary>
        /// <param name="userToUpdate">User data to update</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserView>> UpdateUserProfileAsync(UserToUpdate userToUpdate, string idFromIdentity);

        /// <summary>
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserView>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity);

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

        /// <summary>
        ///  Asynchronously update user password
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        Task<Result<UserView>> UpdatePasswordAsync(UserPasswordToChange userInfo);

        ///// <summary>
        /////  Asynchronously update user email
        ///// </summary>
        ///// <param name="userInfo">User data to update</param>
        //Task<Result<UserView>> UpdateEmailAsync(UserEmailToChange userInfo);
    }
}
