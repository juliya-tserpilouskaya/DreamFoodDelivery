﻿using DreamFoodDelivery.Common;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    // FIX REG
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenSecret _tokenSecret;
        private readonly IEmailSenderService _emailSender;
        private readonly IEmailBuilder _emailBuilder;
        private readonly ITokenValidator _tokenValidator;
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public IdentityService(UserManager<AppUser> userManager,
                               IUserService userService,
                               IAdminService adminService,
                               RoleManager<IdentityRole> roleManager, 
                               TokenSecret tokenSecret, 
                               IEmailSenderService emailSender,
                               IEmailBuilder emailBuilder,
                               ITokenValidator tokenValidator)
        {
            _userManager = userManager;
            _userService = userService;
            _adminService = adminService;
            _roleManager = roleManager;
            _tokenSecret = tokenSecret;
            _emailSender = emailSender;
            _emailBuilder = emailBuilder;
            _tokenValidator = tokenValidator;
        }

        /// <summary>
        /// Create Admin using constant data
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> CreateAdminAsync(CancellationToken cancellationToken = default)
        {
            string password = SuperAdminData.PASSWORD;
            var user = new AppUser
            {
                Email = SuperAdminData.EMAIL,
                UserName = SuperAdminData.USER_NAME,
                PersonalDiscount = 0,
                Role = AppIdentityConstants.ADMIN,
                IsEmailConfirmed = false
            };
            var createUser = await _userManager.CreateAsync(user, password);
            if (!createUser.Succeeded)
            {
                return Result.Fail(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }
            await _userManager.AddToRoleAsync(user, user.Role);
            var profile = await _userService.CreateAccountAsyncById(user.Id, cancellationToken);
            var admin = await _userManager.FindByEmailAsync(user.Email);
            var result = await _adminService.ConfirmEmailAsync(admin.Id);
            if (result.IsSuccess)
            {
                admin.IsEmailConfirmed = true;
                await _userManager.UpdateAsync(admin);
            }
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

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return Result<UserWithToken>.Quite<UserWithToken>(ExceptionConstants.USER_ALREADY_EXIST);
            }

            var newUser = new AppUser
            {
                Email = user.Email,
                UserName = user.Email,
                PersonalDiscount = 0,
                Role = AppIdentityConstants.USER,
                IsEmailConfirmed = false
            };

            var createUser = await _userManager.CreateAsync(newUser, user.Password);
            if (!createUser.Succeeded)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(createUser.Errors.Select(_ => _.Description).Join("\n"));
            }
            await _userManager.AddToRoleAsync(newUser, AppIdentityConstants.USER);

            var profile = await _userService.CreateAccountAsyncById(newUser.Id, cancellationToken);
            var token = await GenerateToken(newUser);
            UserWithToken result = new UserWithToken()
            {
                UserView = profile.Data,
                UserToken = token.Data,
                RefreshToken = GenerateRefreshToken(),
                ExpiresIn = NumberСonstants.REFRESH
            };
            var isRefreshToketAdded = await _userService.AddRefreshTokenAsync(result.RefreshToken, profile.Data.UserDTO.IdFromIdentity);
            if (isRefreshToketAdded.IsError)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(isRefreshToketAdded.Message);
            }
            await _emailBuilder.SendConfirmMessage(newUser, user.CallBackUrl, cancellationToken);
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

            var isUserDeleted = await _userService.DeleteUserByIdFromIdentityAsync(user.Id, cancellationToken);
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
            
            var profile = await _userService.GetUserByIdFromIdentityAsync(user.Id, cancellationToken);
            var token = await GenerateToken(user);
            if (profile.IsError || string.IsNullOrEmpty(token.Data))
            {
                return Result<UserWithToken>.Fail<UserWithToken>(profile.Message + NotificationConstans.TOKEN_IS_NULL);
            }

            UserWithToken result = new UserWithToken()
            {
                UserView = profile.Data,
                UserToken = token.Data,
                RefreshToken = GenerateRefreshToken(),
                ExpiresIn = NumberСonstants.REFRESH
            };

            var isRefrashToketAdded = await _userService.AddRefreshTokenAsync(result.RefreshToken, profile.Data.UserDTO.IdFromIdentity);

            if (isRefrashToketAdded.IsError)
            {
                return Result<UserWithToken>.Fail<UserWithToken>(isRefrashToketAdded.Message);
            }

            return Result<UserWithToken>.Ok(result);
        }

        /// <summary>
        /// Genetate token
        /// </summary>
        /// <param name="user"></param>
        [LoggerAttribute]
        private async Task<Result<string>> GenerateToken(AppUser user)
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
                Expires = DateTime.UtcNow.AddHours(NumberСonstants.TOKEN_TIME_HOUR),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Ok(tokenHandler.WriteToken(token));
        }

        private static string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<Result<UserWithToken>> ExchangeRefreshToken(TokenRefresh request, CancellationToken cancellationToken = default)
        {
            var cp = _tokenValidator.GetPrincipalFromToken(request.AccessToken, _tokenSecret.SecretString);

            if (cp != null)
            {
                var userId = cp.Claims.First(c => c.Type == "id");

                var isRemovedRefreshToken = await _userService.DeleteRefreshTokenAsync(request.RefreshToken, userId.Value, cancellationToken); // delete the token we've exchanged
                if (isRemovedRefreshToken.IsSuccess)
                {
                    var user = await _userManager.FindByIdAsync(userId.Value);
                    if (user == null)
                    {
                        return Result<UserWithToken>.Quite<UserWithToken>(ExceptionConstants.USER_DOES_NOT_EXISTS);
                    }

                    var profile = await _userService.GetUserByIdFromIdentityAsync(user.Id, cancellationToken);
                    var token = await GenerateToken(user);

                    if (profile.IsError || string.IsNullOrEmpty(token.Data))
                    {
                        return Result<UserWithToken>.Fail<UserWithToken>(profile.Message + NotificationConstans.TOKEN_IS_NULL);
                    }

                    UserWithToken result = new UserWithToken()
                    {
                        UserView = profile.Data,
                        UserToken = token.Data,
                        RefreshToken = GenerateRefreshToken(),
                        ExpiresIn = NumberСonstants.REFRESH
                    };

                    var isRefreshToketAdded = await _userService.AddRefreshTokenAsync(result.RefreshToken, profile.Data.UserDTO.IdFromIdentity); // add the new one
                    if (isRefreshToketAdded.IsError)
                    {
                        return Result<UserWithToken>.Fail<UserWithToken>(isRefreshToketAdded.Message);
                    }
                    return Result<UserWithToken>.Ok(result);
                }

                return Result<UserWithToken>.Fail<UserWithToken>(isRemovedRefreshToken.Message);
            }

            return Result<UserWithToken>.Quite<UserWithToken>(ExceptionConstants.CLAIM_PRINCIPAL_ERROR);
        }
    }
}
