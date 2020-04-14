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
        /// <param name="userId">Existing user Id to add</param>
        Task<Result<BasketDTO>> AddDishAsync(string dishId, string userId);

        /// <summary>
        /// Removes dish from basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to remove</param>
        /// <param name="userId">Existing user Id to remove</param>
        Task<Result<BasketDTO>> RemoveDishByIdAsync(string dishId, string userId);
    }
}
