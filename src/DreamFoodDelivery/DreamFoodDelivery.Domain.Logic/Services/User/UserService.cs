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
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        IEmailSenderService _emailSenderService;

        public UserService(IMapper mapper, DreamFoodDeliveryContext context, UserManager<User> userManager, IEmailSenderService emailSenderService)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
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
            var userToAdd = _mapper.Map<UserDB>(newProfile);
            userToAdd.BasketId = Guid.NewGuid();
            _context.Users.Add(userToAdd);
            BasketDB basketToAdd = new BasketDB() { Id = userToAdd.BasketId, UserId = userToAdd.Id};
            _context.Baskets.Add(basketToAdd);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                UserDB userAfterAdding = await _context.Users.Where(_ => _.IdFromIdentity == userToAdd.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userToAdd.IdFromIdentity);

                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterAdding)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterAdding)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserView>.Fail<UserView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Get User Profile by idFromIdentity. Helper
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
                    return Result<UserProfile>.Fail<UserProfile>($"User was not found");
                }
                return Result<UserProfile>.Ok(_mapper.Map<UserProfile>(profile));
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Get User identityId. Used in Identity service. Aslo helper - move it
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
                    return Result<UserView>.Fail<UserView>($"User was not found");
                }
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userDB.IdFromIdentity);

                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userDB)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
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
                return Result<UserView>.Fail<UserView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Remove user by idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result> DeleteUserByIdFromIdentityAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.IdFromIdentity == idFromIdentity);
            if (user is null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                _context.Users.Remove(user); //Revise the lecture about the database. A moment about deleting information.
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete User. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete User. {ex.Message}"));
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
            User usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>($"User was not found");
            }
            usersIdentity.Address = userToUpdate.Address;
            usersIdentity.PhoneNumber = userToUpdate.PhoneNumber;
            usersIdentity.Name = userToUpdate.Name;
            usersIdentity.Surname = userToUpdate.Surname;
            try
            {
                await _userManager.UpdateAsync(usersIdentity);
                var userProfile = await GetUserProfileByIdFromIdentityAsync(usersIdentity.Id);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == usersIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update user password
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdatePasswordAsync(UserPasswordToChange userInfo, CancellationToken cancellationToken = default)
        {
            User usersIdentity = await _userManager.FindByIdAsync(userInfo.IdFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>($"User was not found");
            }
            try
            {
                await _userManager.ChangePasswordAsync(usersIdentity, userInfo.CurrentPassword, userInfo.NewPassword);

                var userProfile = await GetUserProfileByIdFromIdentityAsync(userInfo.IdFromIdentity);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userInfo.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update user email
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateEmailAsync(UserEmailToChange userInfo, CancellationToken cancellationToken = default)
        {
            User usersIdentity = await _userManager.FindByIdAsync(userInfo.IdFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>($"User was not found");
            }
            try
            {
                var myToken = await _userManager.GenerateChangeEmailTokenAsync(usersIdentity, userInfo.NewEmail);
                await _userManager.ChangeEmailAsync(usersIdentity, userInfo.NewEmail, myToken);
                usersIdentity.UserName = userInfo.NewEmail;
                await _userManager.UpdateAsync(usersIdentity);
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userInfo.IdFromIdentity);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userInfo.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> ConfirmEmailAsync(string idFromIdentity, CancellationToken cancellationToken = default)
        {
            User usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>($"User was not found");
            }
            try
            {
                var myToken = await _userManager.GenerateEmailConfirmationTokenAsync(usersIdentity);
                await _userManager.ConfirmEmailAsync(usersIdentity, myToken);

                var userProfile = await GetUserProfileByIdFromIdentityAsync(idFromIdentity);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously confirms user email
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result> ConfirmEmailSendAsync(string idFromIdentity)
        {
            User usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result.Fail($"User was not found");
            }
            try
            {
                var myToken = await _userManager.GenerateEmailConfirmationTokenAsync(usersIdentity);
                //EmailSenderService.SendMail(usersIdentity.Email, "Registration", myToken);
                _emailSenderService.SendMailResult(usersIdentity.Email, "Registration", myToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result.Fail($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail($"Cannot update model. {ex.Message}");
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
            User usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result<UserView>.Fail<UserView>($"User was not found");
            }
            try
            {
                await _userManager.ConfirmEmailAsync(usersIdentity, token);

                var userProfile = await GetUserProfileByIdFromIdentityAsync(idFromIdentity);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (userProfile.IsError)
                {
                    UserView failProfile = new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                    };
                    return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
                }
                UserView view = new UserView()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
                };
                return Result<UserView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously returs IsEmailConfirmedAsync
        /// </summary>
        /// <param name="idFromIdentity">User id to confirm email</param>
        [LoggerAttribute]
        public async Task<Result> IsEmailConfirmedAsync(string idFromIdentity)
        {
            User usersIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            if (usersIdentity is null)
            {
                return Result.Fail($"User was not found");
            }
            try
            {
                var isIt = await _userManager.IsEmailConfirmedAsync(usersIdentity);
                if (isIt)
                {
                    return Result.Ok();
                }
                return Result.NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result.Fail($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail($"Cannot update model. {ex.Message}");
            }
        }
    }
}
