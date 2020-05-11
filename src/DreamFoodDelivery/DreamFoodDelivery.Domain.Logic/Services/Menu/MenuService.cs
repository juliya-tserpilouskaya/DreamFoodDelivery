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
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync(cancellationToken);
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
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                views.Add(viewItem);
            }

            return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
        }

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        [LoggerAttribute]
        public async Task<Result<DishView>> GetByIdAsync(string dishId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(dishId); 
            try
            {
                DishDB dish = await _context.Dishes.Where(_ => _.Id == id).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView view = _mapper.Map<DishView>(dish);
                view.TagList = new HashSet<TagToAdd>();
                foreach (var item in dish.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Name.Contains(name)).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
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
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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

        ///// <summary>
        /////  Asynchronously returns dish by category. Id must be verified 
        ///// </summary>
        ///// <param name="categoryString">Dish category</param>
        //[LoggerAttribute]
        //public async Task<Result<IEnumerable<DishView>>> GetByCategoryAsync(string categoryString, CancellationToken cancellationToken = default)
        //{
        //    var category = double.Parse(categoryString); //make tryParse
        //    try
        //    {
        //        var dishes = await  _context.Dishes.Where(_ => _.Category == category).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
        //        if (!dishes.Any())
        //        {
        //            return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
        //        }

        //        List<DishView> views = new List<DishView>();
        //        foreach (var dish in dishes)
        //        {
        //            DishView viewItem = _mapper.Map<DishView>(dish);
        //            viewItem.TagList = new HashSet<TagToAdd>();
        //            foreach (var item in dish.DishTags)
        //            {
        //                var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        //                viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
        //            }
        //            views.Add(viewItem);
        //        }
        //        return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>($"Source is null. {ex.Message}");
        //    }
        //}

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="lowerPrice">Dish lower price</param>
        /// <param name="upperPrice">Dish upper price</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetByPriceAsync(double lowerPrice, double upperPrice, CancellationToken cancellationToken = default)
        {           
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Cost >= lowerPrice).Where(_ => _.Cost <= upperPrice).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
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
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetSalesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Sale > 0).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
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
                        var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        ///  Asynchronously get dishes by tag index. Id must be verified 
        /// </summary>
        /// <param name="tagName">Existing tag</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetByTagIndexAsync(/*int tagIndex*/ string tagName, CancellationToken cancellationToken = default)
        {
            //TagDB tag = await _context.Tags.Where(_ => _.IndexNumber == tagIndex).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            TagDB tag = await _context.Tags.Where(_ => _.TagName == tagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            var dishTags = await _context.DishTags.Where(_ => _.TagId  == tag.Id).AsNoTracking().ToListAsync(cancellationToken);
            if (!dishTags.Any())
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
            }

            List<DishView> views = new List<DishView>();
            foreach (var dishTag in dishTags)
            {
                DishDB dish = await _context.Dishes.Where(_ => _.Id == dishTag.DishId).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView viewItem = _mapper.Map<DishView>(dish);

                viewItem.TagList = new HashSet<TagToAdd>();
                foreach (var item in dish.DishTags)
                {
                    var tagItem = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    viewItem.TagList.Add(_mapper.Map<TagToAdd>(tagItem));
                }
                views.Add(viewItem);
            }

            return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
        }
    }
}
