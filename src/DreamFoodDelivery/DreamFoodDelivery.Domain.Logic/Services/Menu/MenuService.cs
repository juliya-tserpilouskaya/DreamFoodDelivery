using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class MenuService : IMenuService
    {
        public Task<Result<Dish>> AddAsync(Dish dish)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dish>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetByCostAsync(string cost)
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetByСonditionAsync(string condition)
        {
            throw new NotImplementedException();
        }

        public Task<Dish> GetSalesAsync()
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Dish>> UpdateAsync(Dish dish)
        {
            throw new NotImplementedException();
        }
    }
}
