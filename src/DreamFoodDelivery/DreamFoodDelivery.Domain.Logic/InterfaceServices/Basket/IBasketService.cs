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
        /// <summary>
        ///  Asynchronously add dish to basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to add</param>
        /// <param name="quantity">Dish quantity to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        Task<Result<BasketView>> AddUpdateDishAsync(string dishId, string userIdFromIdentity, int quantity);

        /// <summary>
        /// Asynchronously removes dish from basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to remove</param>
        /// <param name="userIdFromIdentity">Existing user Id to remove</param>
        Task<Result<BasketView>> RemoveDishByIdAsync(string dishId, string userIdFromIdentity);

        /// <summary>
        /// Asynchronously get all dishes by user Id. Id must be verified
        /// </summary>
        /// <param name="userIdFromIdentity"></param>
        Task<Result<BasketView>> GetAllDishesByUserIdAsync(string userIdFromIdentity);

        /// <summary>
        ///  Asynchronously remove all dishes from basket by user Id
        /// </summary>
        Task<Result> RemoveAllByUserIdAsync(string userIdFromIdentity);
    }
}
