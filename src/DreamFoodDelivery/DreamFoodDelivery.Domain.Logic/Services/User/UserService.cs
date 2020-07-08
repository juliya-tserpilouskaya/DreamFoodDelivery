using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    //------------------------------------------------------------------
    // Created using template, 4/5/2020 9:44:16 PM
    //------------------------------------------------------------------
    public class UserService : IUserService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailSender;
        private readonly IEmailBuilder _emailBuilder;

        public UserService(IMapper mapper, 
                           DreamFoodDeliveryContext context, 
                           UserManager<AppUser> userManager, 
                           IEmailSenderService emailSender, 
                           IEmailBuilder emailBuilder)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailBuilder = emailBuilder;
        }

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> CreateAccountAsyncById(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserGeneration newProfile = new UserGeneration()
            {
                IdFromIdentity = userIdFromIdentity
            };
            UserDB userToAdd = _mapper.Map<UserDB>(newProfile);
            userToAdd.BasketId = Guid.NewGuid();
            _context.Users.Add(userToAdd);
            BasketDB basketToAdd = new BasketDB() { Id = userToAdd.BasketId, UserId = userToAdd.Id};
            _context.Baskets.Add(basketToAdd);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                UserView view = GetUserByIdFromIdentityAsync(userToAdd.IdFromIdentity).Result.Data;
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        /// Get User Profile by idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        [LoggerAttribute]
        public async Task<Result<UserProfile>> GetUserProfileByIdFromIdentityAsync(string idFromIdentity)
        {
            try
            {
                var profile = await _userManager.FindByIdAsync(idFromIdentity);

                if (profile is null)
                {
                    return Result<UserProfile>.Fail<UserProfile>(ExceptionConstants.USER_WAS_NOT_FOUND);
                }
                return Result<UserProfile>.Ok(_mapper.Map<UserProfile>(profile));
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        /// Get User identityId
        /// </summary>
        /// <param name="idFromIdentity"></param>
        [LoggerAttribute]
        public async Task<Result<UserView>> GetUserByIdFromIdentityAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            try
            {
                var userDB = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userDB is null)
                {
                    return Result<UserView>.Fail<UserView>(ExceptionConstants.USER_WAS_NOT_FOUND);
                }
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userDB.IdFromIdentity);

                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userDB)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userDB)
                };
                return Result<UserView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        /// Remove user by idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result> DeleteUserByIdFromIdentityAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(_ => _.IdFromIdentity == idFromIdentity);
            if (user is null)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND));
            }
            try
            {
                var basket = await _context.Baskets.Where(_ => _.UserId == user.Id).AsNoTracking().FirstOrDefaultAsync();
                _context.Baskets.Remove(basket);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_USER + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_USER + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously update user profile
        /// </summary>
        /// <param name="userToUpdate">User data to update</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateUserProfileAsync(UserToUpdate userToUpdate, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            usersIdentity.Address = userToUpdate.Address;
            usersIdentity.PhoneNumber = userToUpdate.PhoneNumber;
            usersIdentity.Name = userToUpdate.Name;
            usersIdentity.Surname = userToUpdate.Surname;
            try
            {
                await _userManager.UpdateAsync(usersIdentity);

                UserView view = GetUserByIdFromIdentityAsync(usersIdentity.Id).Result.Data;
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously update user password
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdatePasswordAsync(UserPasswordToChange userInfo, CancellationToken cancellationToken = default)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(userInfo.IdFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            try
            {
                await _userManager.ChangePasswordAsync(usersIdentity, userInfo.CurrentPassword, userInfo.NewPassword);
                UserView view = GetUserByIdFromIdentityAsync(userInfo.IdFromIdentity).Result.Data;
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously update user email
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateEmailAsync(UserEmailToChange userInfo, CancellationToken cancellationToken = default)
        {
            AppUser userIdentity = await _userManager.FindByIdAsync(userInfo.IdFromIdentity);
            if (userIdentity is null)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
            }
            AppUser emailCheck = await _userManager.FindByEmailAsync(userInfo.NewEmail);
            if(!(emailCheck is null))
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.USER_ALREADY_EXIST);
            }
            try
            {
                
                var myToken = await _userManager.GenerateChangeEmailTokenAsync(userIdentity, userInfo.NewEmail);
                await _userManager.ChangeEmailAsync(userIdentity, userInfo.NewEmail, myToken);
                userIdentity.UserName = userInfo.NewEmail;
                var sendEmailBefore = await _emailSender.SendEmailAsync(userIdentity.Email, EmailConstants.EMAIL_SUBJECT, userInfo.NewEmail + EmailConstants.EMAIL_MESSAGE, cancellationToken);
                if (sendEmailBefore.IsError)
                {
                    return Result<UserView>.Fail<UserView>(sendEmailBefore.Message);
                }
                await _userManager.UpdateAsync(userIdentity);
                UserView view = GetUserByIdFromIdentityAsync(userInfo.IdFromIdentity).Result.Data;
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result> ConfirmEmailSendAsync(string idFromIdentity)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result.Fail(ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
            }
            try
            {
                var myToken = await _userManager.GenerateEmailConfirmationTokenAsync(usersIdentity);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        /// <param name="token">User token to confirm email</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> ConfirmEmailGetAsync(string idFromIdentity, string token, CancellationToken cancellationToken = default)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
            }
            try
            {
                await _userManager.ConfirmEmailAsync(usersIdentity, token);
                UserView view = GetUserByIdFromIdentityAsync(idFromIdentity).Result.Data;
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously returs IsEmailConfirmedAsync
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result> IsEmailConfirmedAsync(string idFromIdentity)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result.Fail(ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
            }
            try
            {
                var isIt = await _userManager.IsEmailConfirmedAsync(usersIdentity);
                if (isIt)
                {
                    return Result.Ok();
                }
                return Result.Quite();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        /// <param name="token"></param>
        [LoggerAttribute]
        public async Task<Result> ConfirmEmailByLinkAsync(string idFromIdentity, string token)
        {
            AppUser userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (userIdentity is null)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.IDENTITY_USER_WAS_NOT_FOUND);
            }
            try
            {
                var decodedTokenBytes = Convert.FromBase64String(token);
                string decodedTokenString = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(userIdentity, decodedTokenString);
                if (result.Succeeded)
                {
                    userIdentity.IsEmailConfirmed = true;
                    await _userManager.UpdateAsync(userIdentity);
                }
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(x => x.Description).Join("\n"));
            }
            catch (Exception ex)
            {
                return Result<UserView>.Fail<UserView>(ex.Message);
            }
        }

        /// <summary>
        /// Send email for reset password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [LoggerAttribute]
        public async Task<Result> ForgotPasswordAsync(PasswordRecoveryRequest request, CancellationToken cancellationToken = default)
        {
            AppUser user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND));
            }

            return await _emailBuilder.SendPasswordResetMessageAsync(user, request.CallBackUrl, cancellationToken);
        }

        /// <summary>
        /// Reser password and send email about it
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [LoggerAttribute]
        public async Task<Result> ResetPasswordAsync(PasswordRecoveryInfo userInfo, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userInfo.UserId);
            if (user == null)
            {
                return Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND); 
            }

            var decodedTokenBytes = Convert.FromBase64String(userInfo.Token);
            string decodedTokenString = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(user, decodedTokenString, userInfo.Password);
            if (result.Succeeded)
            {
                return await _emailBuilder.SendEmailWithLinkAsync(user.Email, EmailConstants.PASSWORD_RESET_SUBJECT, EmailConstants.PASSWORD_RESET_LINK, EmailConstants.PASSWORD_RESET_MESSAGE, userInfo.CallBackUrl, cancellationToken);
            }
            else
            {
                return Result.Fail(result.Errors.Select(x => x.Description).Join("\n"));
            }
        }

        public async Task<Result> AddRefreshTokenAsync(string refreshToken, string idFromIdentity)
        {
            try
            {
                var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.IdFromIdentity == idFromIdentity);
                _context.RefreshTokens.Add(new RefreshToken(refreshToken, DateTime.UtcNow.AddDays(NumberСonstants.DAYS_TO_EXPIRE), user.Id));
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_SAVE_CHANGES + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_SAVE_CHANGES + ex.Message));
            }
        }

        public async Task<Result> DeleteRefreshTokenAsync(string refreshToken, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    return Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND);
                }
                var refreshTokens = await _context.RefreshTokens.Where(_ => _.UserId == user.Id).AsNoTracking().ToListAsync(cancellationToken);

                if (refreshTokens.Any(_=> _.Token == refreshToken && _.Active))
                {
                    _context.RefreshTokens.Remove(refreshTokens.First(t => t.Token == refreshToken));
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(Result.Ok());
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_SAVE_CHANGES + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_SAVE_CHANGES + ex.Message));
            }
            return await Task.FromResult(Result.Fail(ExceptionConstants.INVALID_REFRESH_TOKEN));
        }
    }
}
