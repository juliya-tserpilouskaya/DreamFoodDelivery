using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class DishService : IDishService
    {
        public Task<Result<DishDTO>> AddDishAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DishDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DishDTO> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
