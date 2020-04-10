using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///  Asynchronously add new account
        /// </summary>
        /// <param name="user">New user to add</param>
        public async Task<Result<UserDTO>> CreateAccountAsync(UserDTO user)
        {
            var userToAdd = _mapper.Map<UserDB>(user);
            userToAdd.Id = Guid.NewGuid();
            _context.Users.Add(userToAdd);

            try
            {
                await _context.SaveChangesAsync();
                UserDB thingAfterAdding = await _context.Users.Where(_ => _.Id == userToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(thingAfterAdding));
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

            _context.Users.Add(userToAdd);

            try
            {
                await _context.SaveChangesAsync();
                UserDB thingAfterAdding = await _context.Users.Where(_ => _.IdFromIdentity == userToAdd.IdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(thingAfterAdding));
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
        /// Asynchronously returns all users
        /// </summary>
        public async Task<Result<IEnumerable<UserDTO>>> GetAllAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            if (!users.Any())
            {
                return Result<IEnumerable<UserDTO>>.Fail<IEnumerable<UserDTO>>("No users found");
            }
            return Result<IEnumerable<UserDTO>>.Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<Result<UserDTO>> GetByIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            try
            {
                var thing = await _context.Users.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (thing is null)
                {
                    return Result<UserDTO>.Fail<UserDTO>($"User was not found");
                }
                return Result<UserDTO>.Ok(_mapper.Map<UserDTO>(thing));
            }
            catch (ArgumentNullException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously remove user by Id. Id must be verified
        /// </summary>
        /// <param name="userId">ID of existing user</param>
        public async Task<Result> RemoveByIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);

            if (user is null)
            {
                return await Task.FromResult(Result.Fail("User was not found"));
            }
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

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
        ///  Asynchronously update user
        /// </summary>
        /// <param name="user">Existing user to update</param>
        public async Task<Result<UserDTO>> UpdateAsync(UserDTO user)
        {
            UserDB thingForUpdate = _mapper.Map<UserDB>(user);
            _context.Entry(thingForUpdate).Property(c => c.Role).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.EMail).IsModified = true;
            _context.Entry(thingForUpdate).Property(c => c.UserInfo).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return Result<UserDTO>.Ok(user);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<UserDTO>.Fail<UserDTO>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
