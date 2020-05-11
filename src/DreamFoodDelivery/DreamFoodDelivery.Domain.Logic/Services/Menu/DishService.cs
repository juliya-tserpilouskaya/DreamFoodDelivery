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
        [LoggerAttribute]
        public async Task<Result<DishView>> AddAsync(DishToAdd dish, CancellationToken cancellationToken = default)
        {
            DishDB dishToAdd = _mapper.Map<DishDB>(dish);
            dishToAdd.Added = DateTime.UtcNow;
            _context.Dishes.Add(dishToAdd);
            try
            {
                //foreach (var item in dish.TagIndexes)
                foreach (var item in dish.TagNames)
                {
                    //TagDB tag = await _context.Tags.Where(_ => _.IndexNumber == item.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    TagDB tag = await _context.Tags.Where(_ => _.TagName == item.TagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    if (tag is null)
                    {
                        var tagAfter = await _tagService.AddTagDBAsync(item);
                        dishToAdd.DishTags.Add(new DishTagDB { TagId = tagAfter.Data.Id, DishId = dishToAdd.Id });
                    }
                    else
                    {
                        dishToAdd.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = dishToAdd.Id });
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                DishDB thingAfterAdding = await _context.Dishes.Where(_ => _.Id == dishToAdd.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView view = _mapper.Map<DishView>(thingAfterAdding);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in thingAfterAdding.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    view.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                return Result<DishView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishView>.Fail<DishView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<DishView>.Fail<DishView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishView>.Fail<DishView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        [LoggerAttribute]
        public async Task<Result<DishView>> UpdateAsync(DishToUpdate dish, CancellationToken cancellationToken = default)
        {
            DishDB thingForUpdate = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == dish.Id);

            var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync(cancellationToken);
            _context.DishTags.RemoveRange(dishTagList);
            thingForUpdate.DishTags.Clear();
            //_context.Entry(thingForUpdate).Collection(c => c.DishTags).IsModified = true;
            await _context.SaveChangesAsync(cancellationToken);

            thingForUpdate = _mapper.Map<DishDB>(dish);
            thingForUpdate.Modified = DateTime.Now;

            _context.Entry(thingForUpdate).Property(c => c.Name).IsModified = true;
            //_context.Entry(thingForUpdate).Property(c => c.Category).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Composition).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Description).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Cost).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Weigh).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Sale).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Modified).IsModified = true;

            foreach (var item in dish.TagIndexes)
            {
                //TagDB tag = await _context.Tags.Where(_ => _.IndexNumber == item.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                TagDB tag = await _context.Tags.Where(_ => _.TagName == item.TagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (tag is null)
                {
                    var tagAfter = await _tagService.AddTagDBAsync(item);
                    thingForUpdate.DishTags.Add(new DishTagDB { TagId = tagAfter.Data.Id, DishId = thingForUpdate.Id });
                }
                else
                {
                    thingForUpdate.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = thingForUpdate.Id });
                }
                //thingForUpdate.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = thingForUpdate.Id });
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                DishDB thingAfterAdding = await _context.Dishes.Where(_ => _.Id == thingForUpdate.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView view = _mapper.Map<DishView>(thingAfterAdding);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in thingAfterAdding.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    view.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                return Result<DishView>.Ok(_mapper.Map<DishView>(view));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<DishView>.Fail<DishView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<DishView>.Fail<DishView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string dishId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(dishId);
            var dish = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);
            var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync(cancellationToken);
            _context.DishTags.RemoveRange(dishTagList);
            dish.DishTags.Clear();
            if (dish is null)
            {
                return await Task.FromResult(Result.Fail("Dish was not found"));
            }
            try
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync(cancellationToken);
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
        ///  Asynchronously remove all dishes 
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync(cancellationToken);
            if (dishes is null)
            {
                return await Task.FromResult(Result.Fail("Dishes were not found"));
            }
            try
            {
                foreach (var dish in dishes)
                {
                    var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync(cancellationToken);
                    _context.DishTags.RemoveRange(dishTagList);
                    dish.DishTags.Clear();
                    if (dish is null)
                    {
                        return await Task.FromResult(Result.Fail("Dish was not found"));
                    }
                    _context.Dishes.Remove(dish);
                    await _context.SaveChangesAsync(cancellationToken);
                }
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
    }
}
