using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IOrderService
    {
        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        Task<Result<IEnumerable<OrderView>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously get order by order Id. Id must be verified 
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        Task<Result<OrderView>> GetByIdAsync(string orderId);

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<IEnumerable<OrderView>>> GetByUserIdAsync(string userId);

        /// <summary>
        ///  Asynchronously add new thing
        /// </summary>
        /// <param name="order">New order to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        Task<Result<OrderView>> AddAsync(OrderToAdd order, string userIdFromIdentity);

        /// <summary>
        ///  Asynchronously remove order by Id. Id must be verified
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        Task<Result> RemoveByIdAsync(string orderId);

        /// <summary>
        ///  Asynchronously remove all orders 
        /// </summary>
        Task<Result> RemoveAll();

        /// <summary>
        ///  Asynchronously update order
        /// </summary>
        /// <param name="order">Existing order to update</param>
        Task<Result<OrderView>> UpdateAsync(OrderToUpdate order);

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserId(string userId);

        /// <summary>
        ///  Asynchronously update order status
        /// </summary>
        /// <param name="order">New order status</param>
        Task<Result<OrderView>> UpdateOrderStatusAsync(OrderToStatusUpdate order);
    }
}
