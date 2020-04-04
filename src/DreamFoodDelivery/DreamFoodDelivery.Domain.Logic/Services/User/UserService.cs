using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class UserService : IUserService
    {
        public Task<Result<User>> CreateAccountAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
