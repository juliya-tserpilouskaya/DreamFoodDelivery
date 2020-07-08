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
    public class AdminService : IAdminService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailSender;
        IUserService _userService;

        public AdminService(IMapper mapper, DreamFoodDeliveryContext context, UserManager<AppUser> userManager, IUserService userService, IEmailSenderService emailSender)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _userService = userService;
            _emailSender = emailSender;
        }
        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<UserView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var usersDB = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
            var usersIdentity = await _userManager.Users.ToListAsync(cancellationToken); //clean... or leave, because an interesting test is written.

            if (!usersDB.Any() || !usersIdentity.Any())
            {
                return Result<IEnumerable<UserView>>.Fail<IEnumerable<UserView>>(ExceptionConstants.USERS_WERE_NOT_FOUND);
            }

            List<UserView> users = new List<UserView>();
            foreach (var user in usersDB)
            {
                //var userIdentity = usersIdentity.Where(_ => _.Id == user.IdFromIdentity).Select(_ => _).FirstOrDefault();
                var userView = CollectUserView(user);
                users.Add(userView.Result);
            }
            return Result<IEnumerable<UserView>>.Ok(_mapper.Map<IEnumerable<UserView>>(users));
        }

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            try
            {
                var userDB = await _context.Users.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userDB is null)
                {
                    return Result<UserView>.Fail<UserView>(ExceptionConstants.USER_WAS_NOT_FOUND);
                }
                UserView view = CollectUserView(userDB).Result;
                return Result<UserView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserView>.Fail<UserView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateUserPersonalDiscountAsync(int personalDiscount, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            userIdentity.PersonalDiscount = personalDiscount; 
            try
            {
                await _userManager.UpdateAsync(userIdentity);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                UserView view = CollectUserView(userAfterUpdate).Result;
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
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var userDB = await _context.Users.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);
            var basket = await _context.Baskets.Where(_ => _.UserId == id).AsNoTracking().FirstOrDefaultAsync();
            if (userDB is null || basket is null)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND));
            }
            try
            {
                _context.Baskets.Remove(basket);
                _context.Users.Remove(userDB);
                
                await _context.SaveChangesAsync(cancellationToken);
                var userIdentity = await _userManager.FindByIdAsync(userDB.IdFromIdentity);
                var result = await _userManager.DeleteAsync(userIdentity); 
                if (!result.Succeeded)
                {
                    return await Task.FromResult(Result.Fail(result.Errors.Select(x => x.Description).Join("\n")));
                }
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
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="idFromIdentity">Existing user ID</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result> ChangeRoleAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);

            if (userIdentity == null)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND));
            }
            try
            {
                if (await _userManager.IsInRoleAsync(userIdentity, AppIdentityConstants.USER))
                {
                    userIdentity.Role = AppIdentityConstants.ADMIN;
                    await _userManager.UpdateAsync(userIdentity);
                    await _userManager.AddToRoleAsync(userIdentity, AppIdentityConstants.ADMIN);
                    await _userManager.RemoveFromRoleAsync(userIdentity, AppIdentityConstants.USER);
                    var result =  await _emailSender.SendEmailAsync(userIdentity.Email, EmailConstants.ROLE_SUBJECT, EmailConstants.ROLE_MESSAGE_FOR_ADMIN, cancellationToken);
                    if (result.IsError)
                    {
                        return Result.Quite(NotificationConstans.SOMETHING_WRONG_WITH_EMAIL + result.Message);
                    }
                    return await Task.FromResult(Result.Ok());
                }
                else if (await _userManager.IsInRoleAsync(userIdentity, AppIdentityConstants.ADMIN))
                {
                    userIdentity.Role = AppIdentityConstants.USER;
                    await _userManager.UpdateAsync(userIdentity);
                    await _userManager.AddToRoleAsync(userIdentity, AppIdentityConstants.USER);
                    await _userManager.RemoveFromRoleAsync(userIdentity, AppIdentityConstants.ADMIN);
                    var result = await _emailSender.SendEmailAsync(userIdentity.Email, EmailConstants.ROLE_SUBJECT, EmailConstants.ROLE_MESSAGE_FOR_USER, cancellationToken);
                    if (result.IsError)
                    {
                        return Result.Quite(NotificationConstans.SOMETHING_WRONG_WITH_EMAIL + result.Message);
                    }
                    return await Task.FromResult(Result.Ok());
                }
                return await Task.FromResult(Result.Quite(NotificationConstans.SOMETHING_WRONG_WITH_ROLES));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_CHANGE_ROLE + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_CHANGE_ROLE + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result> ConfirmEmailAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            AppUser usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result.Fail(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            try
            {
                var myToken = await _userManager.GenerateEmailConfirmationTokenAsync(usersIdentity);
                var result = await _userManager.ConfirmEmailAsync(usersIdentity, myToken);
                if (result.Succeeded)
                {
                    usersIdentity.IsEmailConfirmed = true;
                    await _userManager.UpdateAsync(usersIdentity);
                }
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
        /// Collect user db info and profile data to view model
        /// </summary>
        /// <param name="user">User db data</param>
        ///// <param name="userIdentity">User identity data</param>
        /// <returns></returns>
        public async Task<UserView> CollectUserView(UserDB user)
        {
            UserView view = new UserView();
            view.UserDTO = _mapper.Map<UserDTO>(user);
            var userProfile = await _userService.GetUserProfileByIdFromIdentityAsync(user.IdFromIdentity);
            if (userProfile.IsError)
            {
                view.UserProfile = null;
            }
            else
            {
                view.UserProfile = userProfile.Data;
            }
            return view;
        }
    }
}
