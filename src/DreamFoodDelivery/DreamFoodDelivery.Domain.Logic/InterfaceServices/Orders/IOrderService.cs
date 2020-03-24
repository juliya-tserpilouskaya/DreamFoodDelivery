using DreamFoodDelivery.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Orders
{
    public interface IOrderService
    {
        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Asynchronously returns oeder by id
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns></returns>
        Task<Order> GetByIdAsync(string id);

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="order">New order</param>
        /// <returns></returns>
        Task<Result<Order>> AddAsync(Order order);

        /// <summary>
        /// Updates order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns></returns>
        Task<Result<Order>> UpdateAsync(Order order);

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
        Task<IEnumerable<Order>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Updates order
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        Task<Result<Order>> UpdateByUserIdAsync(Order order, string userId);

        /// <summary>
        /// Removes comments by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);
    }
}
