using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IUserService
    {
        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        Task<Result<UserView>> CreateAccountAsyncById(string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get User Profile by idFromIdentity. Helper
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result<UserProfile>> GetUserProfileByIdFromIdentityAsync(string idFromIdentity);

        /// <summary>
        /// Get User idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result<UserView>> GetUserByIdFromIdentityAsync(string idFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update user
        /// </summary>
        /// <param name="userToUpdate">User data to update</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserView>> UpdateUserProfileAsync(UserToUpdate userToUpdate, string idFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove User idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        Task<Result> DeleteUserByIdFromIdentityAsync(string idFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update user password
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        Task<Result<UserView>> UpdatePasswordAsync(UserPasswordToChange userInfo, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update user email
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        Task<Result<UserView>> UpdateEmailAsync(UserEmailToChange userInfo, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        Task<Result<UserView>> ConfirmEmailAsync(string idFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        Task<Result> ConfirmEmailSendAsync(string idFromIdentity);

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        /// <param name="token">User token to confirm email</param>
        Task<Result<UserView>> ConfirmEmailGetAsync(string idFromIdentity, string token, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously returs IsEmailConfirmedAsync
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        Task<Result> IsEmailConfirmedAsync(string idFromIdentity);
    }
}
