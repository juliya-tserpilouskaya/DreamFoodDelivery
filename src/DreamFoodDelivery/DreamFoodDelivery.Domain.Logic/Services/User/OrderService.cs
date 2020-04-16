using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
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
        private readonly IMapper _mapper;

        public OrderService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Asynchronously returns all orders
        /// </summary>
        public async Task<Result<IEnumerable<OrderView>>> GetAllAsync()
        {
            var orders = await _context.Orders.AsNoTracking().ToListAsync();
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>("No orders found");
            }
            return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(orders));
        }

        /// <summary>
        ///  Asynchronously get order by order Id. Id must be verified 
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
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
                return Result<OrderView>.Ok(_mapper.Map<OrderView>(order));
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
        public async Task<Result<OrderToAdd>> AddAsync(OrderToAdd order)
        {
            //if info from profile
            var orderToAdd = _mapper.Map<OrderDB>(order);

            _context.Orders.Add(orderToAdd);

            try
            {
                await _context.SaveChangesAsync();

                OrderDB orderAfterAdding = await _context.Orders.Where(_ => _.UserId == orderToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();

                return Result<OrderToAdd>.Ok(_mapper.Map<OrderToAdd>(orderAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderToAdd>.Fail<OrderToAdd>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderToAdd>.Fail<OrderToAdd>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<OrderToAdd>.Fail<OrderToAdd>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update order
        /// </summary>
        /// <param name="order">Existing order to update</param>
        /// <param name="userId">ID guid of existing order</param>
        public async Task<Result<OrderToUpdate>> UpdateAsync(OrderToUpdate order)
        {
            OrderDB orderForUpdate = _mapper.Map<OrderDB>(order);
            _context.Entry(orderForUpdate).Property(c => c.IsInfoFromProfile).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.Address).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.PersonalDiscount).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.PhoneNumber).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.Name).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.FinaleCost).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.ShippingСost).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.OrderTime).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.DeliveryTime).IsModified = true;
            _context.Entry(orderForUpdate).Property(c => c.PaymentTime).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return Result<OrderToUpdate>.Ok(order);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<OrderToUpdate>.Fail<OrderToUpdate>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<OrderToUpdate>.Fail<OrderToUpdate>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously get orders by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result<IEnumerable<OrderView>>> GetByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var orders = await _context.Orders.Where(_ => _.UserId == id).Select(_ => _).ToListAsync();
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderView>>.Fail<IEnumerable<OrderView>>("No orders found");
            }
            return Result<IEnumerable<OrderView>>.Ok(_mapper.Map<IEnumerable<OrderView>>(orders));
        }

        /// <summary>
        ///  Asynchronously remove order by Id. Id must be verified
        /// </summary>
        /// <param name="orderId">ID of existing order</param>
        public async Task<Result> RemoveByIdAsync(string orderId)
        {
            Guid id = Guid.Parse(orderId);
            var order = await _context.Orders.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);

            if (order is null)
            {
                return await Task.FromResult(Result.Fail("Order was not found"));
            }
            try
            {
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
        public async Task<Result> RemoveAllAsync()
        {
            var order = await _context.Orders.ToListAsync();
            if (order is null)
            {
                return await Task.FromResult(Result.Fail("Orders were not found"));
            }
            try
            {
                _context.Orders.RemoveRange(order);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete orders. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete orders. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove all orders by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result> RemoveAllByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var order = _context.Orders.Where(_ => _.UserId == id).Select(_ => _);

            if (order is null)
            {
                return await Task.FromResult(Result.Fail("Orders were not found"));
            }
            try
            {
                _context.Orders.RemoveRange(order);
                await _context.SaveChangesAsync();

                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete orders. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete orders. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously update order status
        /// </summary>
        /// <param name="order">New order status</param>
        public async Task<Result<OrderView>> UpdateOrderStatusAsync(OrderToStatusUpdate order)
        {
            
            OrderDB orderForUpdate = _mapper.Map<OrderDB>(order);
            orderForUpdate.UpdateTime = DateTime.Now;
            _context.Entry(orderForUpdate).Property(c => c.Status).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                OrderDB orderAfterUpdate = await _context.Orders.Where(_ => _.UserId == orderForUpdate.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<OrderView>.Ok(_mapper.Map<OrderView>(orderAfterUpdate));
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
