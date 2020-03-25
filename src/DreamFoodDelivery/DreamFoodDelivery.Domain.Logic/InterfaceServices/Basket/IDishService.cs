using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Basket
{
    class IDishService
    {
        /// <summary>
        /// Add dish to basket
        /// </summary>
        /// <param name="dish">New dish</param>
        /// <returns></returns>
        Task<Result<BasketModel>> AddDishAsync(Dish dish);

        /// <summary>
        /// Removes dish from basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);

        /// <summary>
        /// Get dishes from user basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);

        /// <summary>
        /// View dish details
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);
    }
}
