using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    /// <summary>
    /// Menu search
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Get all dishes by request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<IEnumerable<DishView>>> GetAllDishesByRequestAsync(RequestParameters request, CancellationToken cancellationToken);

        /// <summary>
        /// Get all tags from DB
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<TagView>>> GetAllTagsAsync(CancellationToken cancellationToken);
    }
}
