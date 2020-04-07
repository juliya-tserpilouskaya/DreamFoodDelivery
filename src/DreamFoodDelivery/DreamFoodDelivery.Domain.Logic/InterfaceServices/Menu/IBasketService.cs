using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IBasketService
    {
        //all for Admin
        /// <summary>
        /// Asynchronously returns all baskets
        /// </summary>
        Task<Result<IEnumerable<BasketDTO>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously add new basket
        /// </summary>
        /// <param name="basket">New basket to add</param>>
        Task<Result<BasketDTO>> AddAsync(BasketDTO basket);

        //Add/remive dishes here
        /// <summary>
        ///  Asynchronously update basket
        /// </summary>
        /// <param name="basket">Existing basket to update</param>
        Task<Result<BasketDTO>> UpdateAsync(BasketDTO basket);

        /// <summary>
        ///  Asynchronously remove basket by Id. Id must be verified
        /// </summary>
        /// <param name="basketId">ID of existing basket</param>
        Task<Result> RemoveByIdAsync(string basketId);

        //Is it necessary?
        /// <summary>
        ///  Asynchronously remove all basket by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserIdAsync(string userId);
    }
}
