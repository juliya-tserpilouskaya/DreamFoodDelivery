using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class OrderService : IOrderService
    {
        public Task<Result<Order>> AddAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetByUserIdAsync(string userID)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Order>> UpdateAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Order>> UpdateByUserIdAsync(Order order, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
