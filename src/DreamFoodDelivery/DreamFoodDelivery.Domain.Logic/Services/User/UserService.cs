using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
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
                users.Add(
                    new UserView() //Check and delete excess information
                    {
                        UserProfile = userProfile.Data,
                        UserDTO = _mapper.Map<UserDTO>(item),
         
                        IdFromIdentity = item.IdFromIdentity,
                        Email = item2.Email,
                        Address = item2.Address,
                        Phone = item2.PhoneNumber,
                        PersonalDiscount = item2.PersonalDiscount,
                        BasketId = item.BasketId,
                    });
            }
            return Result<IEnumerable<UserView>>.Ok(_mapper.Map<IEnumerable<UserView>>(users));
        }

        ///// <summary>
        /////  Asynchronously add new account
        ///// </summary>
        ///// <param name="user">New user to add</param>
        //public async Task<Result<UserDTO>> CreateAccountAsync(UserDTO user)
        //{
        //    var userToAdd = _mapper.Map<UserDB>(user);
        //    userToAdd.Id = Guid.NewGuid();
        //    _context.Users.Add(userToAdd);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        UserDB thingAfterAdding = await _context.Users.Where(_ => _.Id == userToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
        //        return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(thingAfterAdding));
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Result<UserDTO>.Fail<UserDTO>($"Cannot save model. {ex.Message}");
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return Result<UserDTO>.Fail<UserDTO>($"Cannot save model. {ex.Message}");
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return Result<UserDTO>.Fail<UserDTO>($"Source is null. {ex.Message}");
        //    }
        //}

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        public async Task<Result<UserDTO>> CreateAccountAsyncById(string userIdFromIdentity)
        {
            UserGeneration newProfile = new UserGeneration()
            {
                IdFromIdentity = userIdFromIdentity
            };
            var userToAdd = _mapper.Map<UserDB>(newProfile);
            userToAdd.BasketId = Guid.NewGuid();
            _context.Users.Add(userToAdd);
            BasketDB basketToAdd = new BasketDB() { Id = userToAdd.BasketId, UserId = userToAdd.Id}; // think about it
            _context.Baskets.Add(basketToAdd);
            //userToAdd.BasketId = basketToAdd.Id;
            //_context.Entry(userToAdd).Property(c => c.BasketId).IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
                UserDB userAfterAdding = await _context.Users.Where(_ => _.IdFromIdentity == userToAdd.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(userAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result<UserViewSecondPlan>> GetByIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            try
            {
                var userDB = await _context.Users.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (userDB is null)
                {
                    return Result<UserViewSecondPlan>.Fail<UserViewSecondPlan>($"User was not found");
                }
                var usersIdentity = await _userManager.FindByIdAsync(userDB.IdFromIdentity);
                var userProfile = await GetUserProfileByIdFromIdentityAsync(userDB.IdFromIdentity);

                if (userProfile.IsError)
                {
                    UserViewSecondPlan failProfile = new UserViewSecondPlan()
                    {
                        UserProfile = null,
                        UserDTO = _mapper.Map<UserDTO>(userDB)
                    };
                    return Result<UserViewSecondPlan>.Fail<UserViewSecondPlan>(failProfile + "Identity user was not found");
                }

                UserViewSecondPlan view = new UserViewSecondPlan()
                {
                    UserProfile = userProfile.Data,
                    UserDTO = _mapper.Map<UserDTO>(userDB)
                };
                return Result<UserViewSecondPlan>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserViewSecondPlan>.Fail<UserViewSecondPlan>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  UserService helper - move it! 
        /// </summary>
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
        public async Task<Result<UserDTO>> GetUserByIdFromIdentityAsync(string idFromIdentity)
        {
            try
            {
                var user = await _context.Users.Where(_ => _.IdFromIdentity == idFromIdentity).AsNoTracking().FirstOrDefaultAsync();
                if (user is null)
                {
                    return Result<UserDTO>.Fail<UserDTO>($"Not found");
                }
                return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(user));
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Source is null. {ex.Message}");
            }
        }

        //Revise the lecture about the database. A moment about deleting information.
        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
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
        /// <param name="user">Existing user to update</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        public async Task<Result<UserProfile>> UpdateUserProfileAsync(UserProfile user, string idFromIdentity)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            userIdentity.Address = user.Address;
            userIdentity.Login = user.Login;
            userIdentity.Name = user.Name;
            userIdentity.Surname = user.Surname;
            try
            {
                await _userManager.UpdateAsync(userIdentity);
                return Result<UserProfile>.Ok(_mapper.Map<UserProfile>(userIdentity));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>($"Cannot update model. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        public async Task<Result<UserProfile>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            userIdentity.PersonalDiscount = double.Parse(personalDiscount); //try parse
            try
            {
                await _userManager.UpdateAsync(userIdentity);
                return Result<UserProfile>.Ok(_mapper.Map<UserProfile>(userIdentity));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserProfile>.Fail<UserProfile>($"Cannot update model. {ex.Message}");
            }
        }

        public async Task<Result> ChangeRoleAsync(string idFromIdentity)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);

            if (userIdentity == null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                if (await _userManager.IsInRoleAsync(userIdentity, "User"))
                {
                    await _userManager.AddToRoleAsync(userIdentity, "Admin");
                    await _userManager.RemoveFromRoleAsync(userIdentity, "User");
                }
                if (await _userManager.IsInRoleAsync(userIdentity, "Admin"))
                {
                    await _userManager.AddToRoleAsync(userIdentity, "User");
                    await _userManager.RemoveFromRoleAsync(userIdentity, "Admin");
                }
                return await Task.FromResult(Result.Ok());
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
    }
}
