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
    /// <summary>
    /// Menu search
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;
        public SearchService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all dishes by request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<DishView>>> GetAllDishesByRequestAsync(RequestParameters request, CancellationToken cancellationToken)
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
                    if (!dishTagsList.Any())
                    {
                        return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
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

                if (!string.IsNullOrEmpty(request.Request) && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Name.Contains(request.Request)).ToList();
                }
                if (request.OnSale && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Sale > 0).ToList();
                }
                if (request.LowerPrice > 0 && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Cost >= request.LowerPrice).ToList();
                }
                if (request.UpperPrice > 0 && request.UpperPrice >= request.LowerPrice && dishesList.Any())
                {
                    dishesList = dishesList.Where(_ => _.Cost <= request.UpperPrice).ToList();
                }
                if (!dishesList.Any())
                {
                    return Result<IEnumerable<DishView>>.Fail<IEnumerable<DishView>>("No dishes found");
                }
                List<DishView> views = new List<DishView>();
                foreach (var dish in dishesList)
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
        /// Get all tags from DB
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<TagView>>> GetAllTagsAsync(CancellationToken cancellationToken)
        {
            var tags = await _context.Tags.OrderBy(_ => _.TagName).AsNoTracking().ToListAsync(cancellationToken);
            if (!tags.Any())
            {
                return Result<IEnumerable<TagView>>.Fail<IEnumerable<TagView>>("No tags found");
            }
            return Result<IEnumerable<TagView>>.Ok(_mapper.Map<IEnumerable<TagView>>(tags));
        }
    }
}
