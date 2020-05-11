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
    public interface IAdminService
    {
        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        Task<Result<IEnumerable<UserView>>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<UserView>> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        Task<Result<UserView>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        Task<Result> RemoveByIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="id"></param>
        Task<Result> ChangeRoleAsync(string id);
    }
}
