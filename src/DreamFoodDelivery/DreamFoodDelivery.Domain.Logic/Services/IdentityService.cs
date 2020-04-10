using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        IUserService _service;
        public IdentityService(UserManager<User> userManager, IUserService service, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _service = service;
            _roleManager = roleManager;
        }

        public Task<Result> DeleteAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserDTO>> RegisterAsync(string email, string password)
        {
            string defaultRole = "User";

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return Result<UserDTO>.Fail<UserDTO>("User with this Emai already exist");
            }

            if (!_userManager.Users.Any())
            {
                defaultRole = "Admin";
            }

            var newUser = new User
            {
                Email = email,
                UserName = email,
                Name = defaultRole,
                Address = "my adress",
                PersonalDiscount = 10
            };

            var createUser = await _userManager.CreateAsync(newUser, password);

            if (!createUser.Succeeded)
            {
                return Result<UserDTO>.Fail<UserDTO>(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }

            await _userManager.AddToRoleAsync(newUser, defaultRole);

            var profile = await _service.CreateAccountAsyncById(newUser.Id);
            UserDTO result = new UserDTO()
            {
                Id = profile.Data.Id,
                IdFromIdentity = profile.Data.IdFromIdentity,
                UserInfo = profile.Data.UserInfo,
                UserInfoId = profile.Data.UserInfoId,
                Basket = profile.Data.Basket,
                BasketId = profile.Data.BasketId,
                Orders = profile.Data.Orders,
                Comments = profile.Data.Comments
            };
            return Result<UserDTO>.Ok(result);
        } 
    }
}
