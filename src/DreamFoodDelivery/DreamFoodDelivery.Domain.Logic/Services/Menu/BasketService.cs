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
        ///  Asynchronously add new basket
        /// </summary>
        /// <param name="basket">New basket to add</param>
        public async Task<Result<BasketDTO>> AddAsync(BasketDTO basket)
        {
            var basketToAdd = _mapper.Map<BasketDB>(basket);
            _context.Baskets.Add(basketToAdd);
            try
            {
                await _context.SaveChangesAsync();

                BasketDB basketAfterAdding = await _context.Baskets.Where(_ => _.UserId == basketToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();

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
        /// Asynchronously returns all baskets
        /// </summary>
        public async Task<Result<IEnumerable<BasketDTO>>> GetAllAsync()
        {
            var baskets = await _context.Baskets.AsNoTracking().ToListAsync();
            if (!baskets.Any())
            {
                return Result<IEnumerable<BasketDTO>>.Fail<IEnumerable<BasketDTO>>("No baskets found");
            }
            return Result<IEnumerable<BasketDTO>>.Ok(_mapper.Map<IEnumerable<BasketDTO>>(baskets));
        }

        /// <summary>
        ///  Asynchronously remove all basket by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result> RemoveAllByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var basket = _context.Baskets.Where(_ => _.UserId == id).Select(_ => _);

            if (basket is null)
            {
                return await Task.FromResult(Result.Fail("Basket were not found"));
            }
            try
            {
                _context.Baskets.RemoveRange(basket);
                await _context.SaveChangesAsync();

                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove basket by Id. Id must be verified
        /// </summary>
        /// <param name="basketId">ID of existing basket</param>
        public async Task<Result> RemoveByIdAsync(string basketId)
        {
            Guid id = Guid.Parse(basketId);
            var basket = await _context.Baskets.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);

            if (basket is null)
            {
                return await Task.FromResult(Result.Fail("Basket was not found"));
            }
            try
            {
                _context.Baskets.Remove(basket);
                await _context.SaveChangesAsync();

                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete thing. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete thing. {ex.Message}"));
            }
        }

        //Add/remive dishes here
        /// <summary>
        ///  Asynchronously update basket
        /// </summary>
        /// <param name="basket">Existing basket to update</param>
        public async Task<Result<BasketDTO>> UpdateAsync(BasketDTO basket)
        {
            basket.ModificationTime = DateTime.Now;
            BasketDB basketForUpdate = _mapper.Map<BasketDB>(basket);
            _context.Entry(basketForUpdate).Property(c => c.Dishes).IsModified = true; //Is it ok?
            _context.Entry(basketForUpdate).Property(c => c.ModificationTime).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return Result<BasketDTO>.Ok(basket);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<BasketDTO>.Fail<BasketDTO>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
