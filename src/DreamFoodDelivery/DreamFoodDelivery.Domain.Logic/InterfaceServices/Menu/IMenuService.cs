using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IMenuService
    {
        /// <summary>
        /// Asynchronously returns menu
        /// </summary>
        Task<Result<IEnumerable<DishView>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result<DishView>> GetByIdAsync(string dishId);

        /// <summary>
        ///  Asynchronously returns dish by nam. Id must be verified 
        /// </summary>
        /// <param name="name">Dish name</param>
        Task<Result<IEnumerable<DishView>>> GetByNameAsync(string name);

        /// <summary>
        ///  Asynchronously returns dish by category. Id must be verified 
        /// </summary>
        /// <param name="categoryString">Dish category</param>
        Task<Result<IEnumerable<DishView>>> GetByCategoryAsync(string categoryString);

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="priceString">Dish price</param>
        Task<Result<IEnumerable<DishView>>> GetByPriceAsync(string priceString);

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        Task<Result<IEnumerable<DishView>>> GetSalesAsync();

        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        Task<Result<DishView>> AddAsync(DishToAdd dish);

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        Task<Result<DishView>> UpdateAsync(DishToUpdate dish);

        /// <summary>
        ///  Asynchronously remove dish by Id. Id must be verified
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result> RemoveByIdAsync(string dishId);

        /// <summary>
        ///  Asynchronously remove all dishes 
        /// </summary>
        Task<Result> RemoveAllAsync();
    }
}
