using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class DishService : IDishService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;
        ITagService _tagService;

        public DishService(IMapper mapper, DreamFoodDeliveryContext context, ITagService tagService)
        {
            _context = context;
            _mapper = mapper;
            _tagService = tagService;
        }

        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result<DishView>> AddAsync(DishToAdd dish, CancellationToken cancellationToken = default)
        {
            DishDB dishToAdd = _mapper.Map<DishDB>(dish);
            dishToAdd.Added = DateTime.UtcNow;
            _context.Dishes.Add(dishToAdd);
            try
            {
                await UpdateTagLinks(dish.TagNames, dishToAdd);
                await _context.SaveChangesAsync(cancellationToken);

                DishDB dishAfterAdding = await _context.Dishes.Where(_ => _.Id == dishToAdd.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView view = _mapper.Map<DishView>(dishAfterAdding);
                view.TagList = CollectTagList(dishAfterAdding.DishTags, cancellationToken).Result;
                return Result<DishView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result<DishView>> GetByIdAsync(string dishId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(dishId);
            try
            {
                DishDB dish = await _context.Dishes.IgnoreQueryFilters().Where(_ => _.Id == id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).FirstOrDefaultAsync(cancellationToken);
                if (dish is null)
                {
                    return Result<DishView>.Fail<DishView>(ExceptionConstants.DISH_WAS_NOT_FOUND);
                }
                DishView view = _mapper.Map<DishView>(dish);
                view.TotalCost = Math.Round(view.Price * (1 - view.Sale / 100), 2);
                view.TagList = CollectTagList(dish.DishTags, cancellationToken).Result;
                return Result<DishView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result<DishView>> UpdateAsync(DishToUpdate dish, CancellationToken cancellationToken = default)
        {
            DishDB dishToUpdate = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == dish.Id);
            if (dishToUpdate is null)
            {
                return Result<DishView>.Quite<DishView>(ExceptionConstants.DISH_WAS_NOT_FOUND);
            }
            var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync(cancellationToken);
            _context.DishTags.RemoveRange(dishTagList);
            dishToUpdate.DishTags.Clear();
            await _context.SaveChangesAsync(cancellationToken);
            try
            {
                dishToUpdate = _mapper.Map<DishDB>(dish);
                dishToUpdate.Modified = DateTime.Now;

                _context.Entry(dishToUpdate).Property(c => c.Name).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Composition).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Description).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Price).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Weight).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Sale).IsModified = true;
                _context.Entry(dishToUpdate).Property(c => c.Modified).IsModified = true;

                await UpdateTagLinks(dish.TagNames, dishToUpdate);

                await _context.SaveChangesAsync(cancellationToken);

                DishDB dishAfterAdding = await _context.Dishes.Where(_ => _.Id == dishToUpdate.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView view = _mapper.Map<DishView>(dishAfterAdding);
                view.TagList = CollectTagList(dishAfterAdding.DishTags, cancellationToken).Result;
                return Result<DishView>.Ok(_mapper.Map<DishView>(view));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<DishView>.Fail<DishView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string dishId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(dishId);
            var dish = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).FirstOrDefaultAsync(_ => _.Id == id);
            if (dish is null)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.DISH_WAS_NOT_FOUND));
            }
            
            try
            {
                var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).ToListAsync(cancellationToken);
                _context.DishTags.RemoveRange(dishTagList);
                dish.DishTags.Clear();
                var dishBasketList = await _context.BasketDishes.Where(_ => _.DishId == dish.Id).ToListAsync(cancellationToken);
                _context.BasketDishes.RemoveRange(dishBasketList);
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_DISH + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_DISH + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously remove all dishes 
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync(cancellationToken);
            if (dishes is null)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.DISHES_WERE_NOT_FOUND));
            }
            try
            {
                foreach (var dish in dishes)
                {
                    var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync(cancellationToken);
                    _context.DishTags.RemoveRange(dishTagList);
                    dish.DishTags.Clear();
                    var dishBasketList = await _context.BasketDishes.Where(_ => _.DishId == dish.Id).ToListAsync(cancellationToken);
                    _context.BasketDishes.RemoveRange(dishBasketList);
                    _context.Dishes.Remove(dish);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_DISHES + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_DISHES + ex.Message));
            }
        }

        /// <summary>
        /// Updates tags and dish-tag links
        /// </summary>
        /// <param name="dishTagNames">List of tag names for dish</param>
        /// <param name="dish">Dish</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task UpdateTagLinks(HashSet<TagToAdd> dishTagNames, DishDB dish, CancellationToken cancellationToken = default)
        {
            foreach (var item in dishTagNames)
            {
                TagDB tag = await _context.Tags.IgnoreQueryFilters().Where(_ => _.TagName == item.TagName).Select(_ => _).FirstOrDefaultAsync(cancellationToken);
                if (tag is null)
                {
                    var tagAfter = await _tagService.AddTagDBAsync(item);
                    dish.DishTags.Add(new DishTagDB { TagId = tagAfter.Data.Id, DishId = dish.Id });
                }
                else if (_context.Entry(tag).Property<bool>("IsDeleted").CurrentValue)
                {
                    _context.Entry(tag).Property<bool>("IsDeleted").CurrentValue = false;
                    dish.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = dish.Id });
                }
                else
                {
                    dish.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = dish.Id });
                }
            }
        }

        /// <summary>
        /// Collect dish tags list for view model
        /// </summary>
        /// <param name="dishTags">List of dish tags</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HashSet<TagToAdd>> CollectTagList(HashSet<DishTagDB> dishTags, CancellationToken cancellationToken = default)
        {
            HashSet<TagToAdd> tagList = new HashSet<TagToAdd>();
            foreach (var dishTag in dishTags)
            {
                var tag = await _context.Tags.Where(_ => _.Id == dishTag.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                tagList.Add(_mapper.Map<TagToAdd>(tag));
            }
            return tagList;
        }
    }
}
