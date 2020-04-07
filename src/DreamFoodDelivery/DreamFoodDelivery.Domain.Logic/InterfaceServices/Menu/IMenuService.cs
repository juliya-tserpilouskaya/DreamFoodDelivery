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
        Task<Result<IEnumerable<DishDTO>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously get dish by dish Id. Id must be verified 
        /// </summary>
        /// <param name="dishId">ID of existing dish</param>
        Task<Result<DishDTO>> GetByIdAsync(string dishId);

        /// <summary>
        ///  Asynchronously returns dish by nam. Id must be verified 
        /// </summary>
        /// <param name="name">Dish name</param>
        Task<IEnumerable<DishDTO>> GetByNameAsync(string name);

        /// <summary>
        ///  Asynchronously returns dish by category. Id must be verified 
        /// </summary>
        /// <param name="category">Dish category</param>
        Task<IEnumerable<DishDTO>> GetByCategoryAsync(string category);

        /// <summary>
        ///  Asynchronously returns dish by cost. Id must be verified 
        /// </summary>
        /// <param name="cost">Dish cost</param>
        Task<IEnumerable<DishDTO>> GetByCostAsync(string cost);

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        Task<IEnumerable<DishDTO>> GetSalesAsync();

        /// <summary>
        ///  Asynchronously returns dish by condition. Id must be verified 
        /// </summary>
        /// <param name="condition">Dish condition</param>
        Task<IEnumerable<DishDTO>> GetByСonditionAsync(string condition);

        /// <summary>
        ///  Asynchronously add new dish
        /// </summary>
        /// <param name="dish">New dish to add</param>
        Task<Result<DishDTO>> AddAsync(DishDTO dish);

        /// <summary>
        ///  Asynchronously update dish
        /// </summary>
        /// <param name="dish">Existing dish to update</param>
        Task<Result<DishDTO>> UpdateAsync(DishDTO dish);

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
