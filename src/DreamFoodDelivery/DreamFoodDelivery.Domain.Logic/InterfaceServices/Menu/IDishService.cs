using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IDishService
    {
        /// <summary>
        /// Add dish to basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        Task<Result<DishDTO>> AddDishAsync(string id);

        /// <summary>
        /// Removes dish from basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Get dishes from user basket
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DishDTO>> GetAllAsync();

        /// <summary>
        /// View dish details
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        Task<DishDTO> GetByIdAsync(string id);
    }
}
