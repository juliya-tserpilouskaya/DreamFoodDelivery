using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public UserService(IMapper mapper, DreamFoodDeliveryContext context, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<UserView>>> GetAllAsync()
        {
            var usersDB = await _context.Users.AsNoTracking().ToListAsync();
            var usersIdentity = await _userManager.Users.ToListAsync();

            if (!usersDB.Any() || !usersIdentity.Any())
            {
                return Result<IEnumerable<UserView>>.Fail<IEnumerable<UserView>>("No users found");
            }

            List<UserView> users = new List<UserView>();
            foreach (var item in usersDB)
            {
                var item2 = usersIdentity.Where(_ => _.Id == item.IdFromIdentity).Select(_ => _).FirstOrDefault();
                var userProfile = await GetUserProfileByIdFromIdentityAsync(item.IdFromIdentity);
                if (userProfile.IsError)
                {
                    users.Add(
                    new UserView()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(item)
                    });
                }
                else
                {
                    users.Add(
                    new UserView() //Check and delete excess information
                    {
                        UserProfile = userProfile.Data,
                        UserDTO = _mapper.Map<UserDTO>(item)
                    });
                }
            }
            return Result<IEnumerable<UserView>>.Ok(_mapper.Map<IEnumerable<UserView>>(users));
        }

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> CreateAccountAsyncById(string userIdFromIdentity)
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
                await _context.SaveChangesAsync();

                UserDB userAfterAdding = await _context.Users.Where(_ => _.IdFromIdentity == userToAdd.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
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
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> GetByIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            try
            {
                var userDB = await _context.Users.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
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
        public async Task<Result<UserView>> GetUserByIdFromIdentityAsync(string idFromIdentity)
        {
            try
            {
                var userDB = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).AsNoTracking().FirstOrDefaultAsync();
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

        //Revise the lecture about the database. A moment about deleting information.
        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var userDB = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (userDB is null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                _context.Users.Remove(userDB); //del in DB
                await _context.SaveChangesAsync();
                var userIdentity = await _userManager.FindByIdAsync(userDB.IdFromIdentity);
                var result = await _userManager.DeleteAsync(userIdentity); //Del in Identity db
                if (!result.Succeeded)
                {
                    return await Task.FromResult(Result.Fail(result.Errors.Select(x => x.Description).Join("\n")));
                }
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete user. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete user. {ex.Message}"));
            }
        }

        //Revise the lecture about the database. A moment about deleting information.
        /// <summary>
        /// Remove user by idFromIdentity
        /// </summary>
        /// <param name="idFromIdentity"></param>
        [LoggerAttribute]
        public async Task<Result> DeleteUserByIdFromIdentityAsync(string idFromIdentity)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.IdFromIdentity == idFromIdentity);
            if (user is null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                _context.Users.Remove(user); //Revise the lecture about the database. A moment about deleting information.
                await _context.SaveChangesAsync();
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
        public async Task<Result<UserView>> UpdateUserProfileAsync(UserToUpdate userToUpdate, string idFromIdentity)
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
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == usersIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
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
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            userIdentity.PersonalDiscount = double.Parse(personalDiscount); //try parse
            try
            {
                await _userManager.UpdateAsync(userIdentity);
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userIdentity.Id);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
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
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="id"></param>
        [LoggerAttribute]
        public async Task<Result> ChangeRoleAsync(string idFromIdentity)
        {
            string user = "User";
            string admin = "Admin";
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);

            if (userIdentity == null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                if (await _userManager.IsInRoleAsync(userIdentity, user))
                {
                    userIdentity.Role = admin;
                    await _userManager.UpdateAsync(userIdentity);
                    await _userManager.AddToRoleAsync(userIdentity, admin);
                    await _userManager.RemoveFromRoleAsync(userIdentity, user);
                    return await Task.FromResult(Result.Ok());
                }
                else if (await _userManager.IsInRoleAsync(userIdentity, admin))
                {
                    userIdentity.Role = user;
                    await _userManager.UpdateAsync(userIdentity);
                    await _userManager.AddToRoleAsync(userIdentity, user);
                    await _userManager.RemoveFromRoleAsync(userIdentity, admin);
                    return await Task.FromResult(Result.Ok());
                }
                return await Task.FromResult(Result.Warning("Something wrong with roles"));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot change role. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot change role. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously update user password
        /// </summary>
        /// <param name="userInfo">User data to update</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdatePasswordAsync(UserPasswordToChange userInfo)
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
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userInfo.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
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

        ///// <summary>
        /////  Asynchronously update user email
        ///// </summary>
        ///// <param name="userInfo">User data to update</param>
        //[LoggerAttribute]
        //public async Task<Result<UserView>> UpdateEmailAsync(UserEmailToChange userInfo)
        //{
        //    User usersIdentity = await _userManager.FindByIdAsync(userInfo.IdFromIdentity);
        //    if (usersIdentity is null)
        //    {
        //        return Result<UserView>.Fail<UserView>($"User was not found");
        //    }
        //    try
        //    {
        //        await _userManager.ChangeEmailAsync(usersIdentity, userInfo.NewEmail, userInfo.Token);

        //        var userProfile = await GetUserProfileByIdFromIdentityAsync(userInfo.IdFromIdentity);
        //        UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userInfo.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
        //        if (userProfile.IsError)
        //        {
        //            UserView failProfile = new UserView()
        //            {
        //                UserProfile = null,
        //                UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
        //            };
        //            return Result<UserView>.Fail<UserView>(failProfile + "Identity user was not found");
        //        }
        //        UserView view = new UserView()
        //        {
        //            UserProfile = userProfile.Data,
        //            UserDTO = _mapper.Map<UserDTO>(userAfterUpdate)
        //        };
        //        return Result<UserView>.Ok(view);
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return Result<UserView>.Fail<UserView>($"Cannot update model. {ex.Message}");
        //    }
        //}
    }
}
