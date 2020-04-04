using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class BasketService : IBasketService
    {
        public Task<Result<Basket>> AddAsync(Basket basket)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Basket>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveAllByUserIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Basket>> UpdateAsync(Basket basket)
        {
            throw new NotImplementedException();
        }
    }
}
