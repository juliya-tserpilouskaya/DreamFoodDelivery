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
using DreamFoodDelivery.Domain.View;

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