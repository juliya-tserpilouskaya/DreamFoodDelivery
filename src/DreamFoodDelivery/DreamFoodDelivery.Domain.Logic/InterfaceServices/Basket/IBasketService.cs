using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Basket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Basket
{
    public interface IBasketService
    {
        //all for Admin
        /// <summary>
        /// Asynchronously returns all baskets
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BasketModel>> GetAllAsync();

        /// <summary>
        /// Create basket
        /// </summary>
        /// <param name="basket">New basket</param>
        /// <returns></returns>
        Task<Result<BasketModel>> AddAsync(BasketModel basket);

        /// <summary>
        /// Updates basket
        /// </summary>
        /// <param name="basket">basket</param>
        /// <returns></returns>
        Task<Result<BasketModel>> UpdateAsync(BasketModel basket);

        /// <summary>
        /// Remove basket by id async
        /// </summary>
        /// <param name="id">basket id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        //Is it necessary?
        /// <summary>
        /// Removes basket by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        Task<Result> RemoveAllByUserIdAsync(string id);
    }
}
