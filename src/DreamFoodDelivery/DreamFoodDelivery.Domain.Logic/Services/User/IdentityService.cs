using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Сonstants;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    // FIX REG
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenSecret _tokenSecret;
        private readonly IEmailSenderService _emailSender;
        private readonly IEmailBuilder _emailConfirmation;
        IUserService _service;

        public IdentityService(UserManager<User> userManager,
                               IUserService service, 
                               RoleManager<IdentityRole> roleManager, 
                               TokenSecret tokenSecret, 
                               IEmailSenderService emailSender,
                               IEmailBuilder emailConfirmation)
        {
            _userManager = userManager;
            _service = service;
            _roleManager = roleManager;
            _tokenSecret = tokenSecret;
            _emailSender = emailSender;
            _emailConfirmation = emailConfirmation;
        }

        /// <summary>
        /// Create Admin
        /// </summary>
        /// <param name="user">User registration data</param>
        [LoggerAttribute]
        public async Task<Result> CreateAdminAsync(CancellationToken cancellationToken = default)
        {
            string password = SuperAdminData.PASSWORD;
            var user = new User
            {
                Email = SuperAdminData.EMAIL,
                UserName = SuperAdminData.USER_NAME,
                PersonalDiscount = 0,
                Role = "Admin",
            };
            var createUser = await _userManager.CreateAsync(user, password);
            if (!createUser.Succeeded)
            {
                return Result.Fail(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }
            await _userManager.AddToRoleAsync(user, user.Role);
            var profile = await _service.CreateAccountAsyncById(user.Id, cancellationToken);
            var admin = await _userManager.FindByEmailAsync(user.Email);
            //await _emailConfirmation.MakeConfirmMessage(admin, "http://localhost:4200/confirmation", cancellationToken);
            await _service.ConfirmEmailAsync(admin.Id);
            return Result.Ok();
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">User registration data</param>
        [LoggerAttribute]
        public async Task<Result<UserWithToken>> RegisterAsync(UserRegistration user, CancellationToken cancellationToken = default)
        {
            if (!_userManager.Users.Any())
            {
                var adminResult = CreateAdminAsync(cancellationToken);
                if (adminResult.Result.IsError)
                {
                    return Result<UserWithToken>.Fail<UserWithToken>(ExceptionConstants.UNABLE_TO_CREATE_ADMIN);
                }
            }

            string defaultRole = "User";

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                return Result<UserWithToken>.Quite<UserWithToken>(ExceptionConstants.USER_ALREADY_EXIST);
            }

            if (!_userManager.Users.Any())
            {
                defaultRole = "Admin";
            }

            var newUser = new User
            {
                Email = user.Email,
                UserName = user.Email,
                PersonalDiscount = 0,
                Role = defaultRole
            };

            var createUser = await _userManager.CreateAsync(newUser, user.Password);
            if (!createUser.Succeeded)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }
            await _userManager.AddToRoleAsync(newUser, defaultRole);

            var profile = await _service.CreateAccountAsyncById(newUser.Id, cancellationToken);
            var token = await GenerateToken(newUser);
            UserWithToken result = new UserWithToken()
            {
                UserView = profile.Data,
                UserToken = token.Data               
            };
            return Result<UserWithToken>.Ok(result);
        }

        /// <summary>
        /// Remove user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        [LoggerAttribute]
        public async Task<Result> DeleteAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.USER_DOES_NOT_EXISTS));
            }

            var userCheckPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userCheckPassword)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.WRONG_PASSWORD));
            }

            var isUserDeleted = await _service.DeleteUserByIdFromIdentityAsync(user.Id, cancellationToken);
            if (isUserDeleted.IsError)
            {
                return await Task.FromResult(Result.Fail(isUserDeleted.Message));
            }

            var result = await _userManager.DeleteAsync(user);

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
        [LoggerAttribute]
        public async Task<Result<UserWithToken>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(ExceptionConstants.USER_DOES_NOT_EXISTS);
            }
            
            var userCheckPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userCheckPassword)
            {
                return Result<string>.Fail<UserWithToken>(ExceptionConstants.WRONG_PASSWORD);
            }
            
            var profile = await _service.GetUserByIdFromIdentityAsync(user.Id, cancellationToken);
            var token = await GenerateToken(user);
            if (profile.IsError || string.IsNullOrEmpty(token.Data))
            {
                return Result<UserWithToken>.Fail<UserWithToken>(profile.Message + NotificationConstans.TOKEN_IS_NULL);
            }

            UserWithToken result = new UserWithToken()
            {
                UserView = profile.Data,
                UserToken = token.Data
            };
            return Result<UserWithToken>.Ok(result);
        }

        /// <summary>
        /// Genetate token
        /// </summary>
        /// <param name="user"></param>
        [LoggerAttribute]
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
                Expires = DateTime.UtcNow.AddHours(NumberСonstants.TOKEN_TIME_HOUR), //try todo update (use video example)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Ok(tokenHandler.WriteToken(token));
        }
    }
}
