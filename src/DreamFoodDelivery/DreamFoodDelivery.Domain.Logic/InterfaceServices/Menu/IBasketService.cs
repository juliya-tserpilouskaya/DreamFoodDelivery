using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Models;
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
        /// <returns></returns>
        Task<IEnumerable<Basket>> GetAllAsync();

        /// <summary>
        /// Create basket
        /// </summary>
        /// <param name="basket">New basket</param>
        /// <returns></returns>
        Task<Result<Basket>> AddAsync(Basket basket);

        /// <summary>
        /// Updates basket
        /// </summary>
        /// <param name="basket">basket</param>
        /// <returns></returns>
        Task<Result<Basket>> UpdateAsync(Basket basket);

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
