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
    // Created using template, 4/5/2020 9:44:50 PM
    //------------------------------------------------------------------
    public class BasketService : IBasketService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;

        public BasketService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///  Asynchronously add dish to basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to add</param>
        /// <param name="userId">Existing user Id to add</param>
        public async Task<Result<BasketDTO>> AddDishAsync(string dishId, string userId)
        {
            DishDB dishToAdd = await _context.Dishes.Where(_ => _.Id == Guid.Parse(dishId)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
            var basket = await _context.Baskets.Where(_ => _.UserId == Guid.Parse(userId)).AsNoTracking().FirstOrDefaultAsync();
            basket.Dishes.Add(dishToAdd);
            basket.ModificationTime = DateTime.Now;
            _context.Entry(basket).Property(c => c.Dishes).IsModified = true;
            _context.Entry(basket).Property(c => c.ModificationTime).IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
                var basketAfterAdding = await _context.Baskets.Where(_ => _.UserId == Guid.Parse(userId)).AsNoTracking().FirstOrDefaultAsync();
                return Result<BasketDTO>.Ok(_mapper.Map<BasketDTO>(basketAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Removes dish from basket
        /// </summary>
        /// <param name="dishId">Existing dish Id to remove</param>
        /// <param name="userId">Existing user Id to remove</param>
        public async Task<Result<BasketDTO>> RemoveDishByIdAsync(string dishId, string userId)
        {
            DishDB dishToRemove = await _context.Dishes.Where(_ => _.Id == Guid.Parse(dishId)).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
            var basket = await _context.Baskets.Where(_ => _.UserId == Guid.Parse(userId)).AsNoTracking().FirstOrDefaultAsync();
            basket.Dishes.Remove(dishToRemove);
            basket.ModificationTime = DateTime.Now;
            _context.Entry(basket).Property(c => c.Dishes).IsModified = true;
            _context.Entry(basket).Property(c => c.ModificationTime).IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
                var basketAfterAdding = await _context.Baskets.Where(_ => _.UserId == Guid.Parse(userId)).AsNoTracking().FirstOrDefaultAsync();
                return Result<BasketDTO>.Ok(_mapper.Map<BasketDTO>(basketAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Source is null. {ex.Message}");
            }
        }
    }
}
