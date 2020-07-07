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
    public interface IOrderService
    {
        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        Task<Result<IEnumerable<OrderView>>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get order by order Id. Id must be verified 
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        Task<Result<OrderView>> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<IEnumerable<OrderView>>> GetByUserIdAdminAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        Task<Result<IEnumerable<OrderView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously add new thing
        /// </summary>
        /// <param name="order">New order to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        Task<Result<OrderView>> AddAsync(OrderToAdd order, string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove order by Id. Id must be verified
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        Task<Result> RemoveByIdAsync(string orderId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Remove all orders 
        /// </summary>
        Task<Result> RemoveAll();

        /// <summary>
        ///  Asynchronously remove all orders 
        /// </summary>
        Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update order
        /// </summary>
        /// <param name="order">Existing order to update</param>
        Task<Result<OrderView>> UpdateAsync(OrderToUpdate order, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserId(string userId);

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update order status
        /// </summary>
        /// <param name="order">New order status</param>
        Task<Result> UpdateOrderStatusAsync(OrderToStatusUpdate order, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously return all order statuses
        /// </summary>
        Task<Result<IEnumerable<OrderStatus>>> GetStatuses(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously return all order in status
        /// </summary>
        Task<Result<IEnumerable<OrderView>>> GetOrdersInStatus(string statusName, CancellationToken cancellationToken = default);
    }
}
