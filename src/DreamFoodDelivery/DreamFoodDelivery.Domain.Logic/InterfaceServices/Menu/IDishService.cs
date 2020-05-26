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
    public interface IDishService
    {
        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        Task<Result<DishView>> AddAsync(DishToAdd dish, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result<DishView>> GetByIdAsync(string dishId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        Task<Result<DishView>> UpdateAsync(DishToUpdate dish, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result> RemoveByIdAsync(string dishId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all dishes 
        /// </summary>
        Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default);
    }
}
