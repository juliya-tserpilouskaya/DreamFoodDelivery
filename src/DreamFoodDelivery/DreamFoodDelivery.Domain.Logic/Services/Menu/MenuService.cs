using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Common;
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
        /// Asynchronously returns menu (all dishes)
        /// </summary>
        public async Task<Result<IEnumerable<DishView>>> GetAllAsync()
        {
            var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync();
            if (!dishes.Any())
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
            }

            List<DishView> views = new List<DishView>();
            foreach (var dish in dishes)
            {
                DishView viewItem = _mapper.Map<DishView>(dish);
                viewItem.TagList = new HashSet<TagToAdd>();
                foreach (var item in dish.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                    viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                views.Add(viewItem);
            }

            return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
        }

        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        public async Task<Result<DishView>> AddAsync(DishToAdd dish)
        {
            DishDB dishToAdd = _mapper.Map<DishDB>(dish);
            dishToAdd.Added = DateTime.UtcNow;
            _context.Dishes.Add(dishToAdd);
            try
            {
                foreach (var item in dish.TagIndexes)
                {
                    TagDB tag = await _context.Tags.Where(_ => _.IndexNumber == item.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                    dishToAdd.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = dishToAdd.Id });
                }
                await _context.SaveChangesAsync();

                DishDB thingAfterAdding = await _context.Dishes.Where(_ => _.Id == dishToAdd.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                DishView view = _mapper.Map<DishView>(thingAfterAdding);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in thingAfterAdding.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
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
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        public async Task<Result<DishView>> GetByIdAsync(string dishId)
        {
            Guid id = Guid.Parse(dishId); 
            try
            {
                DishDB dish = await _context.Dishes.Where(_ => _.Id == id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync();
                DishView view = _mapper.Map<DishView>(dish);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in dish.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                    view.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                if (dish is null)
                {
                    return Result<DishView>.Fail<DishView>($"Dish was not found");
                }
                return Result<DishView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<DishView>.Fail<DishView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by name. Id must be verified 
        /// </summary>
        /// <param name="name">Dish name</param>
        public async Task<Result<IEnumerable<DishView>>> GetByNameAsync(string name)
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Name.Contains(name)).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync();
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
                }

                List<DishView> views = new List<DishView>();
                foreach (var dish in dishes)
                {
                    DishView viewItem = _mapper.Map<DishView>(dish);
                    viewItem.TagList = new HashSet<TagToAdd>();
                    foreach (var item in dish.DishTags)
                    {
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                        viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by category. Id must be verified 
        /// </summary>
        /// <param name="categoryString">Dish category</param>
        public async Task<Result<IEnumerable<DishView>>> GetByCategoryAsync(string categoryString)
        {
            var category = double.Parse(categoryString); //make tryParse
            try
            {
                var dishes = await  _context.Dishes.Where(_ => _.Category == category).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync();
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
                }

                List<DishView> views = new List<DishView>();
                foreach (var dish in dishes)
                {
                    DishView viewItem = _mapper.Map<DishView>(dish);
                    viewItem.TagList = new HashSet<TagToAdd>();
                    foreach (var item in dish.DishTags)
                    {
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                        viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="priceString">Dish price</param>
        public async Task<Result<IEnumerable<DishView>>> GetByPriceAsync(string priceString)
        {
            var price = double.Parse(priceString); //make tryParse
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Cost == price).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync();
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
                }

                List<DishView> views = new List<DishView>();
                foreach (var dish in dishes)
                {
                    DishView viewItem = _mapper.Map<DishView>(dish);
                    viewItem.TagList = new HashSet<TagToAdd>();
                    foreach (var item in dish.DishTags)
                    {
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                        viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        public async Task<Result<IEnumerable<DishView>>> GetSalesAsync()
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Sale > 0).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync();
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
                }

                List<DishView> views = new List<DishView>();
                foreach (var dish in dishes)
                {
                    DishView viewItem = _mapper.Map<DishView>(dish);
                    viewItem.TagList = new HashSet<TagToAdd>();
                    foreach (var item in dish.DishTags)
                    {
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
                        viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                    }
                    views.Add(viewItem);
                }
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously remove all dishes 
        /// </summary>
        public async Task<Result> RemoveAllAsync()
        {
            var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync();
            if (dishes is null)
            {
                return await Task.FromResult(Result.Fail("Dishes were not found"));
            }
            try
            {
                foreach (var dish in dishes)
                {
                    var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync();
                    _context.DishTags.RemoveRange(dishTagList);
                    dish.DishTags.Clear();
                    if (dish is null)
                    {
                        return await Task.FromResult(Result.Fail("Dish was not found"));
                    }
                        _context.Dishes.Remove(dish);
                        await _context.SaveChangesAsync();
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

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        public async Task<Result> RemoveByIdAsync(string dishId)
        {
            Guid id = Guid.Parse(dishId);
            var dish = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);
            var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync();
            _context.DishTags.RemoveRange(dishTagList);
            dish.DishTags.Clear();
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
        public async Task<Result<DishView>> UpdateAsync(DishToUpdate dish)
        {
            DishDB thingForUpdate = await _context.Dishes.IgnoreQueryFilters().Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(_ => _.Id == dish.Id);

            var dishTagList = await _context.DishTags.Where(_ => _.DishId == dish.Id).AsNoTracking().ToListAsync();
            _context.DishTags.RemoveRange(dishTagList);
            thingForUpdate.DishTags.Clear();
            //_context.Entry(thingForUpdate).Collection(c => c.DishTags).IsModified = true;
            await _context.SaveChangesAsync();

            thingForUpdate = _mapper.Map<DishDB>(dish);
            thingForUpdate.Modified = DateTime.Now;

            _context.Entry(thingForUpdate).Property(c => c.Name).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Category).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Сomposition).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Description).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Cost).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Weigh).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Sale).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.Modified).IsModified = true;

            foreach (var item in dish.TagIndexes)
            {
                TagDB tag = await _context.Tags.Where(_ => _.IndexNumber == item.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                thingForUpdate.DishTags.Add(new DishTagDB { TagId = tag.Id, DishId = thingForUpdate.Id });
            }

            try
            {
                await _context.SaveChangesAsync();

                DishDB thingAfterAdding = await _context.Dishes.Where(_ => _.Id == thingForUpdate.Id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                DishView view = _mapper.Map<DishView>(thingAfterAdding);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in thingAfterAdding.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync();
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
    }
}
