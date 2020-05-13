﻿using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    //------------------------------------------------------------------
    // Created using template, 4/5/2020 9:44:50 PM
    //------------------------------------------------------------------
    public class BasketService : IBasketService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        IMenuService _menuService;

        public BasketService(IMapper mapper, UserManager<User> userManager, DreamFoodDeliveryContext context, IMenuService menuService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _menuService = menuService;
        }

        /// <summary>
        ///  Asynchronously add dish to basket or update quantity
        /// </summary>
        /// <param name="dishId">Existing dish Id to add</param>
        /// <param name="quantity">Dish quantity to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        [LoggerAttribute]
        public async Task<Result<BasketView>> AddUpdateDishAsync(string dishId, string userIdFromIdentity, int quantity, CancellationToken cancellationToken = default)
        {
            DishDB dishToAdd = await _context.Dishes.Where(_ => _.Id == Guid.Parse(dishId)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (dishToAdd is null)
            {
                return Result<BasketView>.Fail<BasketView>($"Dish was not found");
            }
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            User userIdentity = await _userManager.FindByIdAsync(userIdFromIdentity);
            if (user is null || userIdentity is null)
            {
                return Result<BasketView>.Fail<BasketView>($"User was not found");
            }
            BasketDB basket = await _context.Baskets.Where(_ => _.UserId == user.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (basket is null)
            {
                return Result<BasketView>.Fail<BasketView>($"Basket was not found");
            }

            var connection = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id && _.DishId == Guid.Parse(dishId)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            
            if (connection is null)
            {
                BasketDishDB basketDish = new BasketDishDB() { BasketId = basket.Id, DishId = dishToAdd.Id, Quantity = quantity, DishCost = dishToAdd.Cost };
                _context.BasketDishes.Add(basketDish);
            }
            else
            {
                if (quantity > 0)
                {
                    connection.Quantity = quantity;
                    _context.Entry(connection).Property(c => c.Quantity).IsModified = true;
                }
                else // quantity == 0
                {
                    _context.BasketDishes.Remove(connection);
                }
            }
            
            basket.ModificationTime = DateTime.Now;
            _context.Entry(basket).Property(c => c.ModificationTime).IsModified = true;
            
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                var dishList = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id).AsNoTracking().ToListAsync(cancellationToken);
                BasketView view = _mapper.Map<BasketView>(basket);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<BasketView>.Fail<BasketView>($"Unable to retrieve data"); 
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.BasketCost += dish.Data.Cost * dish.Data.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                view.BasketCost *= userIdentity.PersonalDiscount / 100;
                if (view.BasketCost < DFD_Сonstants.FREE_SHIPPING_BORDER)
                {
                    view.ShippingCost = DFD_Сonstants.DELIVERY_COST;
                }
                return Result<BasketView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously removes dish from basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to remove</param>
        /// <param name="userIdFromIdentity">Existing user Id to remove</param>
        [LoggerAttribute]
        public async Task<Result<BasketView>> RemoveDishByIdAsync(string dishId, string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            User userIdentity = await _userManager.FindByIdAsync(userIdFromIdentity);
            if (user is null || userIdentity is null)
            {
                return Result<BasketView>.Fail<BasketView>($"User was not found");
            }
            BasketDB basket = await _context.Baskets.Where(_ => _.UserId == user.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (basket is null)
            {
                return Result<BasketView>.Fail<BasketView>($"Basket was not found");
            }
            BasketDishDB connection = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id && _.DishId == Guid.Parse(dishId)).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (connection is null)
            {
                return Result<BasketView>.Fail<BasketView>($"Connection was not found");
            }
            basket.ModificationTime = DateTime.Now;
            _context.Entry(basket).Property(c => c.ModificationTime).IsModified = true;
            _context.BasketDishes.Remove(connection);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                var dishList = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id).AsNoTracking().ToListAsync(cancellationToken);
                BasketView view = _mapper.Map<BasketView>(basket);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<BasketView>.Fail<BasketView>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.BasketCost += dish.Data.Cost * dish.Data.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                view.BasketCost *= userIdentity.PersonalDiscount / 100;
                if (view.BasketCost < DFD_Сonstants.FREE_SHIPPING_BORDER)
                {
                    view.ShippingCost = DFD_Сonstants.DELIVERY_COST;
                }
                return Result<BasketView>.Ok(_mapper.Map<BasketView>(view));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously get all dishes by user Id. Id must be verified
        /// </summary>
        /// <param name="userIdFromIdentity"></param>
        [LoggerAttribute]
        public async Task<Result<BasketView>> GetAllDishesByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            User userIdentity = await _userManager.FindByIdAsync(userIdFromIdentity);
            if (user is null || userIdentity is null)
            {
                return Result<BasketView>.Fail<BasketView>($"User was not found");
            }
            BasketDB basket = await _context.Baskets.Where(_ => _.Id == user.BasketId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (basket is null )
            {
                return Result<BasketView>.Fail<BasketView>($"Basket was not found");
            }
            try
            {
                var dishList = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id).AsNoTracking().ToListAsync(cancellationToken);
                BasketView view = _mapper.Map<BasketView>(basket);
                view.Dishes = new HashSet<DishView>();
                foreach (var dishListItem in dishList)
                {
                    var dish = await _menuService.GetByIdAsync(dishListItem.DishId.ToString());
                    if (dish.IsError)
                    {
                        return Result<BasketView>.Fail<BasketView>($"Unable to retrieve data");
                    }
                    dish.Data.Quantity = dishListItem.Quantity;
                    view.BasketCost += dish.Data.Cost * dish.Data.Quantity;
                    view.Dishes.Add(dish.Data);
                }
                view.BasketCost *= userIdentity.PersonalDiscount / 100;
                if (view.BasketCost < DFD_Сonstants.FREE_SHIPPING_BORDER)
                {
                    view.ShippingCost = DFD_Сonstants.DELIVERY_COST;
                }
                return Result<BasketView>.Ok(_mapper.Map<BasketView>(view));
            }
            catch (ArgumentNullException ex)
            {
                return Result<BasketView>.Fail<BasketView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously remove all dishes from basket by user Id
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Result<BasketView>.Fail<BasketView>($"User was not found");
            }
            BasketDB basket = await _context.Baskets.Where(_ => _.UserId == user.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (basket is null)
            {
                return Result<BasketView>.Fail<BasketView>($"Basket was not found");
            }
            var connections = await _context.BasketDishes.Where(_ => _.BasketId == basket.Id).AsNoTracking().ToListAsync(cancellationToken);
            if (!connections.Any())
            {
                return Result<BasketView>.Fail<BasketView>($"Connections was not found");
            }
            basket.ModificationTime = DateTime.Now;
            _context.Entry(basket).Property(c => c.ModificationTime).IsModified = true;
            _context.BasketDishes.RemoveRange(connections);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                BasketView view = _mapper.Map<BasketView>(basket);
                return Result<BasketView>.Ok(_mapper.Map<BasketView>(view));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dishes. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete dishes. {ex.Message}"));
            }
        }
    }
}
