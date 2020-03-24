using DreamFoodDelivery.Domain.Basket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Basket
{
    public interface IBasketService
    {
        //for Admin
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
        /// Asynchronously returns oeder by id
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns></returns>
        Task<BasketModel> GetByIdAsync(string id);

        

        /// <summary>
        /// Updates order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns></returns>
        Task<Result<BasketModel>> UpdateAsync(BasketModel order);

        /// <summary>
        /// Removes order by id async
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Removes all orders from database
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Asynchronously returns order by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        Task<IEnumerable<BasketModel>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Updates order
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        Task<Result<BasketModel>> UpdateByUserIdAsync(BasketModel order, string userId);

        /// <summary>
        /// Removes comments by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);
    }
}
