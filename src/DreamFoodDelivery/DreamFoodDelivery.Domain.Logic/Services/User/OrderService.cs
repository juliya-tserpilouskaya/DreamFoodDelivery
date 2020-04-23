using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IMapper _mapper;
        IMenuService _menuService;

        public OrderService(IMapper mapper, UserManager<User> userManager, DreamFoodDeliveryContext context, IMenuService menuService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _menuService = menuService;
        }

        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetAllAsync()
        {
            try
            {
                var orders = await _context.Orders.AsNoTracking().ToListAsync();
                if (!orders.Any())
                {
                    return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>("No orders found");
                }

                List<OrderView> views = new List<OrderView>();
                foreach (var order in orders)
                {
                    OrderView viewItem = _mapper.Map<OrderView>(order);
                    var dishList = await _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync();
                    viewItem.Dishes = new HashSet<DishView>();
                    foreach (var dishListItem in dishList)
                    {
                        var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                        if (dish.IsError)
                        {
                            return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>($"Unable to retrieve data");
                        }
                        dish.Data.Quantity = dishListItem.Quantity;
                        viewItem.Dishes.Add(dish.Data);
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>($"Source is null. {ex.Message}");
            }

        }

        /// <summary>
        ///  Asynchronously get order by order Id. Id must be verified 
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> GetByIdAsync(string orderId)
        {
            Guid id = Guid.Parse(orderId);
            try
            {
                var order = await _context.Orders.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (order is null)
                {
                    return Result<OrderView>.Fail<OrderView>($"Order was not found");
                }
                OrderView view = _mapper.Map<OrderView>(order);
                var dishList = await _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync();
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously add new order
        /// </summary>
        /// <param name="order">New order to add</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> AddAsync(OrderToAdd order)
        {
            UserDB userDB = await _context.Users.Where(_ => _.BasketId == order.BasketId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
            User userIdentity = await _userManager.FindByIdAsync(userDB.IdFromIdentity);
            if (userDB is null || userIdentity is null)
            {
                return Result<OrderView>.Fail<OrderView>("User not found");
            }

            OrderDB orderToAdd = _mapper.Map<OrderDB>(order);
            if (orderToAdd.IsInfoFromProfile)
            {
                orderToAdd.Address = userIdentity.Address;
                orderToAdd.PersonalDiscount = userIdentity.PersonalDiscount;
                orderToAdd.PhoneNumber = userIdentity.PhoneNumber;
                orderToAdd.Name = userIdentity.Name;
            }
            orderToAdd.UserId = userDB.Id;
            orderToAdd.Status = Enum.GetName(typeof(OrderStatuses), 0);
            orderToAdd.UpdateTime = DateTime.Now;
            _context.Orders.Add(orderToAdd);

            var connection = await _context.BasketDishes.Where(_ => _.BasketId == order.BasketId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
            connection.OrderId = orderToAdd.Id;
            connection.BasketId = Guid.Empty;
            _context.Entry(connection).Property(c => c.OrderId).IsModified = true;
            _context.Entry(connection).Property(c => c.BasketId).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                OrderDB orderAfterAdding = await _context.Orders.Where(_ => _.Id == orderToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                var dishList = await _context.BasketDishes.Where(_ => _.OrderId == orderAfterAdding.Id).AsNoTracking().ToListAsync();
                OrderView view = _mapper.Map<OrderView>(orderAfterAdding);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update order
        /// </summary>
        /// <param name="order">Existing order to update</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> UpdateAsync(OrderToUpdate order)
        {
            OrderDB orderForUpdate = await _context.Orders.Where(_ => _.Id == order.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
            if (DateTime.Now < orderForUpdate.UpdateTime.Value.AddMinutes(15))///////////////////////////////////////////////////////////////////
            {
                orderForUpdate = _mapper.Map<OrderDB>(order);
                orderForUpdate.UpdateTime = DateTime.Now;
                _context.Entry(orderForUpdate).Property(c => c.Address).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.PhoneNumber).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.Name).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.ShippingСost).IsModified = true;
                _context.Entry(orderForUpdate).Property(c => c.UpdateTime).IsModified = true;
                try
                {
                    await _context.SaveChangesAsync();
                    OrderDB orderAfterAdding = await _context.Orders.Where(_ => _.Id == orderForUpdate.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                    var dishList = await _context.BasketDishes.Where(_ => _.OrderId == orderAfterAdding.Id).AsNoTracking().ToListAsync();
                    OrderView view = _mapper.Map<OrderView>(orderAfterAdding);
                    view.Dishes = new HashSet<DishView>();
                    foreach (var dishListItem in dishList)
                    {
                        var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                        if (dish.IsError)
                        {
                            return Result<OrderView>.Fail<OrderView>($"Unable to retrieve data");
                        }
                        dish.Data.Quantity = dishListItem.Quantity;
                        view.Dishes.Add(dish.Data);
                    }
                    return Result<OrderView>.Ok(view);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Result<OrderView>.Fail<OrderView>($"Cannot update model. {ex.Message}");
                }
                catch (DbUpdateException ex)
                {
                    return Result<OrderView>.Fail<OrderView>($"Cannot update model. {ex.Message}");
                }
            }
            else
            {
                return Result<OrderView>.Warning("Time is over.");
            }
        }

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<OrderView>>> GetByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var orders = await _context.Orders.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync();
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>("No orders found");
            }

            List<OrderView> views = new List<OrderView>();
            foreach (var order in orders)
            {
                OrderView viewItem = _mapper.Map<OrderView>(order);
                var dishList = await _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync();
                viewItem.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result< IEnumerable<OrderView>>.Fail< IEnumerable<OrderView>>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
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
        public async Task<Result> RemoveByIdAsync(string orderId)
        {
            Guid id = Guid.Parse(orderId);
            var order = await _context.Orders.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (order is null)
            {
                return await Task.FromResult(Result.Fail("Order was not found"));
            }

            var dishList = await _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToListAsync();
            try
            {
                _context.BasketDishes.RemoveRange(dishList);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete order. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete order. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders 
        /// </summary>
        public async Task<Result> RemoveAll()
        {
            var orders = _context.Orders.AsNoTracking().ToList();
            if (!orders.Any())
            {
                return await Task.FromResult(Result.Fail("Orders were not found"));
            }
            try
            {
                foreach (var order in orders)
                {
                    if (order is null)
                    {
                        return await Task.FromResult(Result.Fail("Order was not found"));
                    }
                    var dishList = _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToList();
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                }
                _context.SaveChanges();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
            catch (ObjectDisposedException ex)
            {

                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result> RemoveAllByUserId(string userId)
        {
            Guid id = Guid.Parse(userId);
            var orders = _context.Orders.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToList();
            if (orders is null)
            {
                return await Task.FromResult(Result.Fail("Orders were not found"));
            }
            try
            {
                foreach (var order in orders)
                {
                    if (order is null)
                    {
                        return await Task.FromResult(Result.Fail("Order was not found"));
                    }
                    var dishList = _context.BasketDishes.Where(_ => _.OrderId == order.Id).AsNoTracking().ToList();
                    _context.BasketDishes.RemoveRange(dishList);
                    _context.Orders.Remove(order);
                }
                _context.SaveChanges();
                return await Task.FromResult(Result.Ok());
            }
            catch (ObjectDisposedException ex)
            {

                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dish. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously update order status
        /// </summary>
        /// <param name="order">New order status</param>
        [LoggerAttribute]
        public async Task<Result<OrderView>> UpdateOrderStatusAsync(OrderToStatusUpdate order)
        {
            OrderDB orderForUpdate = await _context.Orders.Where(_ => _.Id == order.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
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
                        return Result<OrderView>.Warning("Waiting for payment.");
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
                    return Result<OrderView>.Warning("There is nothing to change.");
            }
            try
            {
                await _context.SaveChangesAsync();
                OrderDB orderAfterUpdate = await _context.Orders.Where(_ => _.Id == orderForUpdate.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                var dishList = await _context.BasketDishes.Where(_ => _.OrderId == orderAfterUpdate.Id).AsNoTracking().ToListAsync();
                OrderView view = _mapper.Map<OrderView>(orderAfterUpdate);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<OrderView>.Fail<OrderView>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                return Result<OrderView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderView>.Fail<OrderView>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
