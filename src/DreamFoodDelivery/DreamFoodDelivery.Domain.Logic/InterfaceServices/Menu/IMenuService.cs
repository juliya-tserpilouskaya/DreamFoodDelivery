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
    public interface IMenuService
    {
        /// <summary>
        /// Asynchronously returns menu
        /// </summary>
        Task<Result<IEnumerable<DishView>>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result<DishView>> GetByIdAsync(string dishId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously returns dish by nam. Id must be verified 
        /// </summary>
        /// <param name="name">Dish name</param>
        Task<Result<IEnumerable<DishView>>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        ///// <summary>
        /////  Asynchronously returns dish by category. Id must be verified 
        ///// </summary>
        ///// <param name="categoryString">Dish category</param>
        //Task<Result<IEnumerable<DishView>>> GetByCategoryAsync(string categoryString, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="lowerPrice">Dish lower price</param>
        /// <param name="upperPrice">Dish upper price</param>
        Task<Result<IEnumerable<DishView>>> GetByPriceAsync(double lowerPrice, double upperPrice, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        Task<Result<IEnumerable<DishView>>> GetSalesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get dishes by tag index. Id must be verified 
        /// </summary>
        /// <param name="tagName">Existing tag</param
        Task<Result<IEnumerable<DishView>>> GetByTagIndexAsync(/*int tagIndex*/ string tagName, CancellationToken cancellationToken = default);
    }
}
