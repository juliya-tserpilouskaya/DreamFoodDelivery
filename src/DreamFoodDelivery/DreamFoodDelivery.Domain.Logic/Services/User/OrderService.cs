using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Сonstants;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    //------------------------------------------------------------------
    // Created using template, 4/5/2020 6:42:39 AM
    //------------------------------------------------------------------
    public class OrderService : IOrderService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSenderService _emailSender;
        private readonly IMapper _mapper;
        IDishService _dishService;

        public OrderService(IMapper mapper, UserManager<User> userManager, DreamFoodDeliveryContext context, IDishService dishService, IEmailSenderService emailSender)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _dishService = dishService;
        }

        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _context.Orders.AsNoTracking().ToListAsync(cancellationToken);
                if (!orders.Any())
                {
                    return Result<IEnumerable<OrderView>>.Quite<IEnumerable<OrderView>>(ExceptionConstants.ORDERS_WERE_NOT_FOUND);
                }

                List<OrderView> views = new List<OrderView>();
                foreach (var order in orders)
                {
                    OrderView viewItem = _mapper.Map<OrderView>(order);
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                    viewItem.Dishes = new HashSet<DishView>();
                    foreach (var dishListItem in dishList)
                    {
                        var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                        if (dish.IsError)
                        {
                            return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                        }
                        dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                        viewItem.Dishes.Add(dish.Data);
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously get order by order Id. Id must be verified 
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(orderId);
            try
            {
                var order = await _context.Orders.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (order is null)
                {
                    return Result<OrderView>.Quite<OrderView>(ExceptionConstants.ORDER_WAS_NOT_FOUND);
                }
                OrderView view = _mapper.Map<OrderView>(order);
                var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                    }
                    dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                    view.Dishes.Add(dish.Data);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously add new order
        /// </summary>
        /// <param name="order">New order to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> AddAsync(OrderToAdd order, string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            User userIdentity = await _userManager.FindByIdAsync(userIdFromIdentity);
            UserDB userDB = await _context.Users.Where(_ => _.IdFromIdentity == userIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            
            if (userDB is null || userIdentity is null)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }

            OrderDB orderToAdd = _mapper.Map<OrderDB>(order);
            if (orderToAdd.IsInfoFromProfile)
            {
                if (userIdentity.Address is null || userIdentity.PhoneNumber is null || userIdentity.Name is null || userIdentity.Surname is null)
                {
                    return Result<OrderView>.Quite<OrderView>(NotificationConstans.FILL_PROFILE);
                }
                orderToAdd.Address = userIdentity.Address;
                orderToAdd.PhoneNumber = userIdentity.PhoneNumber;
                orderToAdd.Name = userIdentity.Name;
                orderToAdd.Surname = userIdentity.Surname;
            }
            orderToAdd.PersonalDiscount = userIdentity.PersonalDiscount;
            orderToAdd.UserId = userDB.Id;
            orderToAdd.Status = Enum.GetName(typeof(OrderStatuses), 0);
            orderToAdd.UpdateTime = DateTime.Now;
            orderToAdd.TotalCost = 0;
            orderToAdd.ShippingCost = 0;

            var connections = await _context.BasketDishes.Where(_ => _.BasketId == userDB.BasketId).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            foreach (var connection in connections)
            {
                orderToAdd.TotalCost = orderToAdd.TotalCost.Value + connection.DishCost.Value * (1 - (connection.Sale.Value / 100)) * connection.Quantity.Value;
            }
            orderToAdd.TotalCost *= (1 - userIdentity.PersonalDiscount / 100);
            orderToAdd.TotalCost = Math.Round(orderToAdd.TotalCost.Value, 2);
            if (orderToAdd.TotalCost < NumberСonstants.FREE_SHIPPING_BORDER)
            {
                orderToAdd.ShippingCost = NumberСonstants.DELIVERY_COST;
            }
            _context.Orders.Add(orderToAdd);

            foreach (var connection in connections)
            {
                connection.OrderId = orderToAdd.Id;
                connection.BasketId = Guid.Empty;
                _context.Entry(connection).Property(c => c.OrderId).IsModified = true;
                _context.Entry(connection).Property(c => c.BasketId).IsModified = true;
            }
            try
            {
                await _context.SaveChangesAsync();
                OrderDB orderAfterAdding = await _context.Orders.Where(_ => _.Id == orderToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                var dishList = await _context.BasketDishes.Where(_ => _.OrderId == orderAfterAdding.Id).AsNoTracking().ToListAsync(cancellationToken);
                OrderView view = _mapper.Map<OrderView>(orderAfterAdding);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                    }
                    dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                    view.Dishes.Add(dish.Data);
                }
                var users = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var user in users)
                {
                    var sendEmailBefore = await _emailSender.SendEmailAsync(user.Email, EmailConstants.NEW_ORDER_SUBJECT, EmailConstants.NEW_ORDER_MESSAGE, cancellationToken);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously update order
        /// </summary>
        /// <param name="order">Existing order to update</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> UpdateAsync(OrderToUpdate order, CancellationToken cancellationToken = default)
        {
            OrderDB orderForUpdate = await _context.Orders.Where(_ => _.Id == Guid.Parse(order.Id)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (DateTime.Now < orderForUpdate.UpdateTime.Value.AddMinutes(NumberСonstants.TIME_TO_CHANGE_ORDER_IN_MINUTES))
            {
                orderForUpdate = _mapper.Map<OrderDB>(order);
                orderForUpdate.UpdateTime = DateTime.Now;
                _context.Entry(orderForUpdate).Property(c => c.Address).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.PhoneNumber).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.Name).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.Surname).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.UpdateTime).IsModified = true;
                try
                {
                    await _context.SaveChangesAsync();
                    OrderDB orderAfterAdding = await _context.Orders.Where(_ => _.Id == orderForUpdate.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == orderAfterAdding.Id).AsNoTracking().ToListAsync(cancellationToken);
                    OrderView view = _mapper.Map<OrderView>(orderAfterAdding);
                    view.Dishes = new HashSet<DishView>();
                    foreach (var dishListItem in dishList)
                    {
                        var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                        if (dish.IsError)
                        {
                            return Result<OrderView>.Fail<OrderView>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                        }
                        dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                        view.Dishes.Add(dish.Data);
                    }
                    return Result<OrderView>.Ok(view);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
                }
                catch (DbUpdateException ex)
                {
                    return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
                }
            }
            else
            {
                return Result<OrderView>.Quite<OrderView>(NotificationConstans.TIME_IS_OVER);
            }
        }

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetByUserIdAdminAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var orders = await _context.Orders.IgnoreQueryFilters().Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderView>>.Quite<IEnumerable<OrderView>>(ExceptionConstants.ORDERS_WERE_NOT_FOUND);
            }

            List<OrderView> views = new List<OrderView>();
            foreach (var order in orders)
            {
                OrderView viewItem = _mapper.Map<OrderView>(order);
                var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                viewItem.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result< IEnumerable<OrderView>>.Fail< IEnumerable<OrderView>>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                    }
                    dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                    viewItem.Dishes.Add(dish.Data);
                }
                views.Add(viewItem);
            }
            return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(views));
        }

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            var orders = await _context.Orders.Where(_ => _.UserId == user.Id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderView>>.Quite<IEnumerable<OrderView>>(ExceptionConstants.ORDERS_WERE_NOT_FOUND);
            }

            List<OrderView> views = new List<OrderView>();
            foreach (var order in orders)
            {
                OrderView viewItem = _mapper.Map<OrderView>(order);
                var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                viewItem.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                    }
                    dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                    viewItem.Dishes.Add(dish.Data);
                }
                views.Add(viewItem);
            }
            return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(views));
        }

        /// <summary>
        ///  Asynchronously remove order by Id. Id must be verified
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string orderId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(orderId);
            var order = await _context.Orders.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (order is null)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.ORDER_WAS_NOT_FOUND));
            }

            var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
            try
            {
                _context.BasketDishes.RemoveRange(dishList);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDER + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDER + ex.Message));
            }
        }

        /// <summary>
        ///  Remove all orders 
        /// </summary>
        public async Task<Result> RemoveAll()
        {
            var orders = _context.Orders.AsNoTracking().ToList();
            if (!orders.Any())
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.ORDERS_WERE_NOT_FOUND));
            }
            try
            {
                foreach (var order in orders)
                {
                    var dishList = _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToList();
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                }
                _context.SaveChanges();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (ObjectDisposedException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result> RemoveAllByUserId(string userId)
        {
            Guid id = Guid.Parse(userId);
            var orders = await _context.Orders.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync();
            if (!orders.Any())
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.ORDERS_WERE_NOT_FOUND));
            }
            try
            {
                foreach (var order in orders)
                {
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync();
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                }
                _context.SaveChanges();
                return await Task.FromResult(Result.Ok());
            }
            catch (ObjectDisposedException ex)
            {

                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously update order status
        /// </summary>
        /// <param name="order">New order status</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> UpdateOrderStatusAsync(OrderToStatusUpdate order, CancellationToken cancellationToken = default)
        {
            OrderDB orderForUpdate = await _context.Orders.Where(_ => _.Id == Guid.Parse(order.Id)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            string orderStatus = Enum.GetName(typeof(OrderStatuses), order.StatusIndex);
            switch (orderStatus)
            {
                case "OnWay":
                    orderForUpdate.Status = "OnWay";
                    orderForUpdate.UpdateTime = DateTime.Now;
                    _context.Entry(orderForUpdate).Property(c => c.Status).IsModified = true;
                    _context.Entry(orderForUpdate).Property(c => c.UpdateTime).IsModified = true;
                    break;
                case "Delivered":
                    if (orderForUpdate.PaymentTime.HasValue)
                    {
                        orderForUpdate.Status = "Delivered";
                        orderForUpdate.DeliveryTime = DateTime.Now;
                        orderForUpdate.UpdateTime = DateTime.Now;
                        _context.Entry(orderForUpdate).Property(c => c.Status).IsModified = true;
                        _context.Entry(orderForUpdate).Property(c => c.DeliveryTime).IsModified = true;
                        _context.Entry(orderForUpdate).Property(c => c.UpdateTime).IsModified = true;
                    }
                    else
                    {
                        return Result<OrderView>.Quite<OrderView>(NotificationConstans.WAITING_FOR_PAYMENT);
                    }
                    break;
                case "Canceled":
                    orderForUpdate.Status = "Canceled";
                    orderForUpdate.UpdateTime = DateTime.Now;
                    _context.Entry(orderForUpdate).Property(c => c.Status).IsModified = true;
                    _context.Entry(orderForUpdate).Property(c => c.UpdateTime).IsModified = true;
                    break;
                case "Paid":
                    orderForUpdate.PaymentTime = DateTime.Now;
                    _context.Entry(orderForUpdate).Property(c => c.PaymentTime).IsModified = true;
                    break;
                default:
                    return Result<OrderView>.Quite<OrderView>(NotificationConstans.NOTHING_TO_CHANGE);
            }
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                OrderDB orderAfterUpdate = await _context.Orders.Where(_ => _.Id == orderForUpdate.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == orderAfterUpdate.Id).AsNoTracking().ToListAsync(cancellationToken);
                OrderView view = _mapper.Map<OrderView>(orderAfterUpdate);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                    }
                    dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                    view.Dishes.Add(dish.Data);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderView>.Fail<OrderView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders 
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders.AsNoTracking().ToListAsync(cancellationToken);
            if (!orders.Any())
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.ORDERS_WERE_NOT_FOUND));
            }
            try
            {
                foreach (var order in orders)
                {
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var orders = await _context.Orders.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync();
            if (!orders.Any())
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.ORDERS_WERE_NOT_FOUND));
            }
            try
            {
                foreach (var order in orders)
                {
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_ORDERS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously return all order statuses
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderStatus>>> GetStatuses(CancellationToken cancellationToken = default)
        {
            try
            {
                HashSet<OrderStatus> orderStatuses = new HashSet<OrderStatus>();

                foreach (int index in Enum.GetValues(typeof(OrderStatuses)))
                {
                    OrderStatus statusItem = new OrderStatus();
                    statusItem.StatusIndex = index;
                    statusItem.StatusName = Enum.GetName(typeof(OrderStatuses), index);
                    orderStatuses.Add(statusItem);
                }
                if (!orderStatuses.Any())
                {
                    return Result<IEnumerable<OrderStatus>>.Quite<IEnumerable<OrderStatus>>(ExceptionConstants.ORDER_STATUSES_WERE_NOT_FOUND);
                }
                return Result<IEnumerable<OrderStatus>>.Ok(_mapper.Map<IEnumerable<OrderStatus>>(orderStatuses));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<OrderStatus>>.Fail<IEnumerable<OrderStatus>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously return all order in status
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetOrdersInStatus(string statusName, CancellationToken cancellationToken = default)
        {
            try
            {
                List<OrderDB> orders = new List<OrderDB>();
                if (statusName == "Paid")
                {
                    orders = await _context.Orders.Where(_ => _.PaymentTime.HasValue).AsNoTracking().ToListAsync(cancellationToken);
                }
                else
                {
                    orders = await _context.Orders.Where(_ => _.Status == statusName).AsNoTracking().ToListAsync(cancellationToken);
                }
                if (!orders.Any())
                {
                    return Result<IEnumerable<OrderView>>.Quite<IEnumerable<OrderView>>(ExceptionConstants.ORDERS_WERE_NOT_FOUND);
                }

                List<OrderView> views = new List<OrderView>();
                foreach (var order in orders)
                {
                    OrderView viewItem = _mapper.Map<OrderView>(order);
                    var dishList = await _context.BasketDishes.IgnoreQueryFilters().Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync(cancellationToken);
                    viewItem.Dishes = new HashSet<DishView>();
                    foreach (var dishListItem in dishList)
                    {
                        var dish = await _dishService.GetByIdAsync(dishListItem.DishId.ToString());
                        if (dish.IsError)
                        {
                            return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.UNABLE_TO_RETRIEVE_DATA);
                        }
                        dish.Data.Quantity = dishListItem.Quantity.GetValueOrDefault();
                        viewItem.Dishes.Add(dish.Data);
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }

        }
    }
}
