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
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        IUserService _userService;

        public AdminService(IMapper mapper, DreamFoodDeliveryContext context, UserManager<User> userManager, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _userService = userService;
        }
        /// <summary>
        /// Asynchronously returns all users
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<UserView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var usersDB = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
            var usersIdentity = await _userManager.Users.ToListAsync(cancellationToken);

            if (!usersDB.Any() || !usersIdentity.Any())
            {
                return Result<IEnumerable<UserView>>.Fail<IEnumerable<UserView>>("No users found");
            }

            List<UserView> users = new List<UserView>();
            foreach (var item in usersDB)
            {
                var item2 = usersIdentity.Where(_ => _.Id == item.IdFromIdentity).Select(_ => _).FirstOrDefault();
                var userProfile = await _userService.GetUserProfileByIdFromIdentityAsync(item2.Id);
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
                    return Result<UserView>.Fail<UserView>($"User was not found");
                }
                var userProfile = await _userService.GetUserProfileByIdFromIdentityAsync(userDB.IdFromIdentity);

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
        ///  Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="idFromIdentity">Existing user ID</param>
        [LoggerAttribute]
        public async Task<Result<UserView>> UpdateUserPersonalDiscountAsync(string personalDiscount, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            var userIdentity = await _userManager.FindByIdAsync(idFromIdentity);
            userIdentity.PersonalDiscount = double.Parse(personalDiscount); //try parse
            try
            {
                await _userManager.UpdateAsync(userIdentity);
                var userProfile = await _userService.GetUserProfileByIdFromIdentityAsync(userIdentity.Id);
                UserDB userAfterUpdate = await _context.Users.Where(_ => _.IdFromIdentity == userIdentity.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string userId, CancellationToken cancellationToken = default)
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
                await _context.SaveChangesAsync(cancellationToken);
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

        /// <summary>
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="idFromIdentity">Existing user ID</param>
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
    }
}
