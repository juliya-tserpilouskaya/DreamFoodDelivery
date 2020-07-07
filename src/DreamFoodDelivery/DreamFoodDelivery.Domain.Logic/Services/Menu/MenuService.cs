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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<DishView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var dishes = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync(cancellationToken);
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Quite<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                }
                List<DishView> views = await CollectViews(dishes);
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }

        }

        public async Task<Result<IEnumerable<DishView>>> GetAllDishesByRequestAsync(RequestParameters request, CancellationToken cancellationToken = default)
        {
            try
            {
                List<DishDB> dishesList = new List<DishDB>();
                if (request.TagsNames.Any())
                {
                    HashSet<DishTagDB> dishTagsList = new HashSet<DishTagDB>();
                    foreach (var tagName in request.TagsNames)
                    {
                        TagDB tag = await _context.Tags.Where(_ => _.TagName == tagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                        if (!(tag is null))
                        {
                            var dishTags = await _context.DishTags.Where(_ => _.TagId == tag.Id).AsNoTracking().ToListAsync(cancellationToken);
                            if (dishTags.Any())
                            {
                                foreach (var dishTag in dishTags)
                                {
                                    if (dishTagsList.Count is 0)
                                    {
                                        dishTagsList.Add(dishTag);
                                    }
                                    var dishTagToCompare = dishTagsList.Where(_ => _.DishId == dishTag.DishId).FirstOrDefault();
                                    if (!dishTagsList.Where(_ => _.DishId == dishTag.DishId).Any())
                                    {
                                        dishTagsList.Add(dishTag);
                                    }
                                }
                            }
                        }
                    }
                    if (!dishTagsList.Any())
                    {
                        return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                    }
                    foreach (DishTagDB dishTag in dishTagsList)
                    {
                        DishDB dish = await _context.Dishes.Where(_ => _.Id == dishTag.DishId).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                        dishesList.Add(dish);
                    }
                }
                else
                {
                    dishesList = await _context.Dishes.Include(c => c.DishTags).ThenInclude(sc => sc.Tag).AsNoTracking().ToListAsync(cancellationToken);
                }

                if (!dishesList.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                }
                if (!string.IsNullOrEmpty(request.Request) && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Name.Contains(request.Request) || _.Composition.Contains(request.Request) || _.Description.Contains(request.Request)).ToList();
                }
                if (request.OnSale && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Sale > 0).ToList();
                }
                if (request.LowerPrice > 0 && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Price >= request.LowerPrice).ToList();
                }
                if (request.UpperPrice > 0 && request.UpperPrice >= request.LowerPrice && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Price <= request.UpperPrice).ToList();
                }

                List<DishView> views = await CollectViews(dishesList);
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by name. Id must be verified 
        ///  !!! Obsolete service. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="name">Dish name</param>
        [LoggerAttribute]
        [Obsolete]
        public async Task<Result<IEnumerable<DishView>>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Name.Contains(name)).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                }

                List<DishView> views = await CollectViews(dishes);
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        ///  !!! Obsolete service. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="lowerPrice">Dish lower price</param>
        /// <param name="upperPrice">Dish upper price</param>
        [LoggerAttribute]
        [Obsolete]
        public async Task<Result<IEnumerable<DishView>>> GetByPriceAsync(double lowerPrice, double upperPrice, CancellationToken cancellationToken = default)
        {           
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Price >= lowerPrice).Where(_ => _.Price <= upperPrice).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                }

                List<DishView> views = await CollectViews(dishes);
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously returns sales
        /// !!! Obsolete service. If necessary, review their return data types and status codes!!!
        /// </summary>
        [LoggerAttribute]
        [Obsolete]
        public async Task<Result<IEnumerable<DishView>>> GetSalesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var dishes = await _context.Dishes.Where(_ => _.Sale > 0).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).ToListAsync(cancellationToken);
                if (!dishes.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
                }

                List<DishView> views = await CollectViews(dishes);
                return Result<IEnumerable<DishView>>.Ok(_mapper.Map<IEnumerable<DishView>>(views));
            }
            catch (ArgumentNullException ex)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously get dishes by tag index. Id must be verified 
        ///  !!! Obsolete service. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="tagName">Existing tag</param>
        [LoggerAttribute]
        [Obsolete]
        public async Task<Result<IEnumerable<DishView>>> GetByTagIndexAsync(string tagName, CancellationToken cancellationToken = default)
        {
            TagDB tag = await _context.Tags.Where(_ => _.TagName == tagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (tag is null)
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.TAG_WAS_NOT_FOUND);
            }
            var dishTags = await _context.DishTags.Where(_ => _.TagId  == tag.Id).AsNoTracking().ToListAsync(cancellationToken);
            if (!dishTags.Any())
            {
                return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>(ExceptionConstants.DISHES_WERE_NOT_FOUND);
            }

            List<DishView> views = new List<DishView>();
            foreach (var dishTag in dishTags)
            {
                DishDB dish = await _context.Dishes.Where(_ => _.Id == dishTag.DishId).Include(c => c.DishTags).ThenInclude(sc => sc.Tag).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                DishView viewItem = _mapper.Map<DishView>(dish);
                viewItem.TotalCost = Math.Round(viewItem.Price * (1 - viewItem.Sale / 100), 2);
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

        /// <summary>
        /// Gets additional data and collects elements dishes views
        /// </summary>
        /// <param name="dishes">Dishes list</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Dishes views</returns>
        private async Task<List<DishView>> CollectViews(List<DishDB> dishes, CancellationToken cancellationToken = default)
        {
            List<DishView> views = new List<DishView>();
            foreach (var dish in dishes)
            {
                DishView viewItem = _mapper.Map<DishView>(dish);
                viewItem.TotalCost = Math.Round(viewItem.Price * (1 - viewItem.Sale / 100), 2);
                viewItem.TagList = new HashSet<TagToAdd>();
                foreach (var item in dish.DishTags)
                {
                    var tag = await _context.Tags.Where(_ => _.Id == item.TagId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                    viewItem.TagList.Add(_mapper.Map<TagToAdd>(tag));
                }
                views.Add(viewItem);
            }
            return views;
        }
    }
}
