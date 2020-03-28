using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices.Menu
{
    public interface IMenuService
    {
        /// <summary>
        /// Asynchronously returns all dishes
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Dish>> GetAllAsync();

        /// <summary>
        /// Asynchronously returns dish by id
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        Task<Dish> GetByIdAsync(string id);

        //Change to search later
        /// <summary>
        /// Asynchronously returns dish by name
        /// </summary>
        /// <param name="name">dish name</param>
        /// <returns></returns>
        Task<Dish> GetByNameAsync(string name);

        /// <summary>
        /// Asynchronously returns dish by category
        /// </summary>
        /// <param name="category">dish category</param>
        /// <returns></returns>
        Task<Dish> GetByCategoryAsync(string category);

        /// <summary>
        /// Asynchronously returns dish by cost
        /// </summary>
        /// <param name="cost">dish cost</param>
        /// <returns></returns>
        Task<Dish> GetByCostAsync(string cost);

        /// <summary>
        /// Asynchronously returns sales
        /// </summary>
        /// <returns></returns>
        Task<Dish> GetSalesAsync();

        /// <summary>
        /// Asynchronously returns dish by condition
        /// </summary>
        /// <param name="condition">dish condition</param>
        /// <returns></returns>
        Task<Dish> GetByСonditionAsync(string condition);

        /// <summary>
        /// Create dish
        /// </summary>
        /// <param name="dish">New dish</param>
        /// <returns></returns>
        Task<Result<Dish>> AddAsync(Dish dish);

        /// <summary>
        /// Updates dish
        /// </summary>
        /// <param name="dish">dish</param>
        /// <returns></returns>
        Task<Result<Dish>> UpdateAsync(Dish dish);

        /// <summary>
        /// Removes dish by id async
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Removes all dishes from database
        /// </summary>
        void RemoveAll();
    }
}
