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
    // Created using template, 4/5/2020 9:45:37 PM
    //------------------------------------------------------------------
    public class MenuService : IMenuService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;

        public MenuService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        public async Task<Result<DishDTO>> AddAsync(DishDTO dish)
        {
            var dishToAdd = _mapper.Map<DishDB>(dish);
            dishToAdd.Id = Guid.NewGuid();
            _context.Dishes.Add(dishToAdd);
            try
            {
                await _context.SaveChangesAsync();
                DishDB thingAfterAdding = await _context.Dishes.Where(_ => _.Id == dishToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<DishDTO>.Ok(_mapper.Map<DishDTO>(thingAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns menu
        /// </summary>
        public async Task<Result<IEnumerable<DishDTO>>> GetAllAsync()
        {
            var dishes = await _context.Dishes.AsNoTracking().ToListAsync();
            if (!dishes.Any())
            {
                return Result<IEnumerable<DishDTO>>.Fail<IEnumerable<DishDTO>>("No dishes found");
            }
            return Result<IEnumerable<DishDTO>>.Ok(_mapper.Map<IEnumerable<DishDTO>>(dishes));
        }

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        public async Task<Result<DishDTO>> GetByIdAsync(string dishId)
        {
            Guid id = Guid.Parse(dishId); 
            try
            {
                var dish = await _context.Dishes.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (dish is null)
                {
                    return Result<DishDTO>.Fail<DishDTO>($"Dish was not found");
                }
                return Result<DishDTO>.Ok(_mapper.Map<DishDTO>(dish));
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by name. Id must be verified 
        /// </summary>
        /// <param name="name">Dish name</param>
        public async Task<IEnumerable<DishDTO>> GetByNameAsync(string name)
        {
            var dish = await _context.Dishes.Where(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<DishDB>, List<DishDTO>>(dish);
        }

        /// <summary>
        ///  Asynchronously returns dish by category. Id must be verified 
        /// </summary>
        /// <param name="category">Dish category</param>
        public async Task<IEnumerable<DishDTO>> GetByCategoryAsync(string category)
        {
            var dish = await _context.Dishes.Where(_ => _.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<DishDB>, List<DishDTO>>(dish);
        }

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="cost">Dish cost</param>
        public async Task<IEnumerable<DishDTO>> GetByCostAsync(string cost)
        {
            var price = double.Parse(cost); //make tryParse
            var dish = await _context.Dishes.Where(_ => _.Cost == price).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<DishDB>, List<DishDTO>>(dish);
        }

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        public async Task<IEnumerable<DishDTO>> GetSalesAsync()
        {
            var dishes = await _context.Dishes.Where(_ => _.Sale > 0).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<DishDB>, List<DishDTO>>(dishes);
        }

        /// <summary>
        ///  Asynchronously returns dish by condition. Id must be verified 
        /// </summary>
        /// <param name="condition">Dish condition</param>
        public async Task<IEnumerable<DishDTO>> GetByСonditionAsync(string condition)
        {
            var dish = await _context.Dishes.Where(_ => _.Category.Equals(condition, StringComparison.OrdinalIgnoreCase)).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<DishDB>, List<DishDTO>>(dish);
        }

        /// <summary>
        ///  Asynchronously remove all dishes 
        /// </summary>
        public async Task<Result> RemoveAllAsync()
        {
            var dish = await _context.Dishes.ToListAsync();
            if (dish is null)
            {
                return await Task.FromResult(Result.Fail("Dishes were not found"));
            }
            try
            {
                _context.Dishes.RemoveRange(dish);
                await _context.SaveChangesAsync();

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
        }

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        public async Task<Result> RemoveByIdAsync(string dishId)
        {
            Guid id = Guid.Parse(dishId);
            var dish = await _context.Dishes.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);

            if (dish is null)
            {
                return await Task.FromResult(Result.Fail("Dish was not found"));
            }
            try
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

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
        }

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        public async Task<Result<DishDTO>> UpdateAsync(DishDTO thing)
        {
            DishDB thingForUpdate = _mapper.Map<DishDB>(thing);
            _context.Entry(thingForUpdate).Property(c => c.Name).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Category).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Сomposition).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Description).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Cost).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Weigh).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Sale).IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
                return Result<DishDTO>.Ok(thing);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<DishDTO>.Fail<DishDTO>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
