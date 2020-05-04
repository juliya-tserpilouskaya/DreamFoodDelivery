using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DreamFoodDelivery.Common;
using Microsoft.Net.Http.Headers;
using FluentValidation.AspNetCore;
using System.Threading;

namespace DreamFoodDelivery.Web.Controllers
{
    /// <summary>
    /// Users other actions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Returns all users stored</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _userService.GetAllAsync(cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get user account by Id
        /// </summary>
        /// <param name="id">User db id</param>
        /// <returns>Returns ID matching user</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _userService.GetByIdAsync(id, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get own profile
        /// </summary>
        /// <returns>Returns users own information</returns>
        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken = default)
        {
            try
            {
                string idFromIdentity = HttpContext.User.Claims.Single(_ => _.Type == "id").Value;
                var result = await _userService.GetUserByIdFromIdentityAsync(idFromIdentity, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update users account by admin
        /// </summary>
        /// <param name="user">User information</param>
        /// <param name="idFromIdentity">Users id from identity</param>
        /// <returns>Returns users information after updating</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut, Route("{idFromIdentity}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateUserProfileByAdmin([FromBody, CustomizeValidator]UserToUpdate user, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            if (user is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserProfileAsync(user, idFromIdentity, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update own account
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>Returns users own information after updating</returns>
        [HttpPut, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateUserProfile([FromBody, CustomizeValidator]UserToUpdate user, CancellationToken cancellationToken = default)
        {
            if (user is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserProfileAsync(user, HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update user personal discount by admin
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <param name="identityId">User ID from identity</param>
        /// <returns>Returns users information after updating</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, Route("{identityId}/discount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> UpdatePersonalDiscount([FromBody]string personalDiscount, string identityId, CancellationToken cancellationToken = default)
        {
            if (personalDiscount is null || identityId is null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserPersonalDiscountAsync(personalDiscount, identityId, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Remove user by userdb id
        /// </summary>
        /// <param name="id">UserDb id</param>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _userService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) 
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) 
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="identityId">Users id from identity</param>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("{identityId}/role_update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ChangeRoleAsync(string identityId)
        {
            if (identityId == null)
            {
                return BadRequest("Invalid id");
            }
            try
            {
                var result = await _userService.ChangeRoleAsync(identityId);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update the user email
        /// </summary>
        /// <param name="userInfo">User information</param>
        /// <returns>Returns user information after changing email</returns>
        [HttpPost, Route("email/change")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ChangeUserEmail([FromBody]UserEmailToChange userInfo, CancellationToken cancellationToken = default)
        {
            if (userInfo is null)
            {
                return BadRequest();
            }
            try
            {
                //var accessToken = HttpContext.Request.Headers[HeaderNames.Authorization];
                var result = await _userService.UpdateEmailAsync(userInfo, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Confirm the user email
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <returns>Returns user information after email confirm</returns>
        [ObsoleteAttribute]
        [HttpPost, Route("email/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmail(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Confirm the user email - send token
        /// </summary>
        /// <returns>Result information</returns>
        [HttpPost, Route("email/send_token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmailSend()
        {
            try
            {
                var result = await _userService.ConfirmEmailSendAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Confirm the user email - get token
        /// </summary>
        /// <param name="token">Token from email</param>
        /// <returns>Returns user information after email confirm</returns>
        [HttpPost, Route("email/get_token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmailGet([FromBody]string token, CancellationToken cancellationToken = default)
        {
            if (token is null)
            {
                return BadRequest();
            }
            try
            {
                var result = await _userService.ConfirmEmailGetAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, token, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="userInfo">User information</param>
        /// <returns>Returns user information after password changing</returns>
        [HttpPost, Route("password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ChasngeUserPassword([FromBody, CustomizeValidator]UserPasswordToChange userInfo, CancellationToken cancellationToken = default)
        {
            if (userInfo is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdatePasswordAsync(userInfo, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}