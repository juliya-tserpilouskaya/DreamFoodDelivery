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
        private readonly TokenSecret _tokenSecret;
        IUserService _service;
        public IdentityService(UserManager<User> userManager, IUserService service, RoleManager<IdentityRole> roleManager, TokenSecret tokenSecret)
        {
            _userManager = userManager;
            _service = service;
            _roleManager = roleManager;
            _tokenSecret = tokenSecret;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">User registration data</param>
        public async Task<Result<UserWithToken>> RegisterAsync(UserRegistration user)
        {
            string defaultRole = "User";

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                return Result<UserWithToken>.Fail<UserWithToken>("User already exist");
            }

            if (!_userManager.Users.Any())
            {
                defaultRole = "Admin";
            }

            var newUser = new User
            {
                Email = user.Email,
                UserName = user.Email
            };

            var createUser = await _userManager.CreateAsync(newUser, user.Password);
            if (!createUser.Succeeded)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }
            await _userManager.AddToRoleAsync(newUser, defaultRole);

            var profile = await _service.CreateAccountAsyncById(newUser.Id);
            var token = await GenerateToken(newUser);

            UserWithToken result = new UserWithToken()
            {
                UserDTO = profile.Data,
                UserToken = token.Data               
            };
            return Result<UserWithToken>.Ok(result);
        }

        /// <summary>
        /// Remove user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async Task<Result> DeleteAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return await Task.FromResult(Result.Fail("User dosn't exists"));
            }

            var userCheckPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userCheckPassword)
            {
                return await Task.FromResult(Result.Fail("Wrong password"));
            }

            var isUserDeleted = await _service.DeleteUserByIdFromIdentityAsync(user.Id); //del in DB
            if (isUserDeleted.IsError)
            {
                return await Task.FromResult(Result.Fail(isUserDeleted.Message));
            }

            var result = await _userManager.DeleteAsync(user); //Del in Identity db

            if (!result.Succeeded)
            {
                return await Task.FromResult(Result.Fail(result.Errors.Select(x => x.Description).Join("\n")));
            }

            return await Task.FromResult(Result.Ok());
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async Task<Result<UserWithToken>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<UserWithToken>.Fail<UserWithToken>("User dosn't exist");
            }
            
            var userCheckPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userCheckPassword)
            {
                return Result<string>.Fail<UserWithToken>("Wrong password");
            }
            
            var profile = await _service.GetUserByIdFromIdentityAsync(user.Id);
            var token = await GenerateToken(user);
            if (profile.IsError || string.IsNullOrEmpty(token.Data))
            {
                return Result<UserWithToken>.Fail<UserWithToken>($"{profile.Message}\n" + $"or token is null"); //spilt it
            }
            UserWithToken result = new UserWithToken()
            {
                UserDTO = profile.Data,
                UserToken = token.Data
            };
            return Result<UserWithToken>.Ok(result);
        }

        /// <summary>
        /// Genetate token
        /// </summary>
        /// <param name="user"></param>
        private async Task<Result<string>> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = Encoding.ASCII.GetBytes(_tokenSecret.SecretString);
            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };
            
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null)
                {
                    continue;
                }

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                    {
                        continue;
                    }
                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), //try todo update (use video example)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Ok(tokenHandler.WriteToken(token));
        }
    }
}
