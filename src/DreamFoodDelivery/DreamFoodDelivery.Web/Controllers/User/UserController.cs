using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DreamFoodDelivery.Common;
using Microsoft.Net.Http.Headers;
using FluentValidation.AspNetCore;
using System.Threading;

namespace DreamFoodDelivery.Web.Controllers
{
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

        //Admin only
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Returns all users stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no users in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Users were found", typeof(IEnumerable<UserView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of users is empty")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _userService.GetAllAsync(cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Get user account by Id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid user id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User was found", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user account
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        [HttpPost, Route("profile")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
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
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Admin
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Asynchronously update user personal discount
        /// </summary>
        /// <param name="personalDiscount">New personal discount</param>
        /// <returns></returns>
        [HttpPost, Route("сhange_discount")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> UpdatePersonalDiscount([FromBody]string personalDiscount, string idFromIdentity, CancellationToken cancellationToken = default)
        {
            if (personalDiscount is null || idFromIdentity is null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserPersonalDiscountAsync(personalDiscount, idFromIdentity, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Remove user by userid
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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
                return result.IsError ? NotFound(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Make admin from user or vice versa
        /// </summary>
        /// <param name="identityId"></param>
        [HttpPost]
        [Route("сhange_role/{identityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> ChangeRoleToAsync(string identityId)
        {
            if (identityId == null)
            {
                return BadRequest("Invalid id");
            }
            try
            {
                var result = await _userService.ChangeRoleAsync(identityId);

                return result.IsError ? NotFound(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user email
        /// </summary>
        /// <param name="userInfo">user</param>
        /// <returns></returns>
        [HttpPost, Route("change_email")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateUserEmail([FromBody]UserEmailToChange userInfo, CancellationToken cancellationToken = default)
        {
            if (userInfo is null /*|| !ModelState.IsValid*/)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //var accessToken = HttpContext.Request.Headers[HeaderNames.Authorization];
                var result = await _userService.UpdateEmailAsync(userInfo, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user email
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("email")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmail(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user email
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("email/sendToken")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmailSend()
        {
            try
            {
                var result = await _userService.ConfirmEmailSendAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user email
        /// </summary>
        /// <param name="token">user token</param>
        /// <returns></returns>
        [HttpPost, Route("email/get/{token}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> ConfirmUserEmailGet(string token, CancellationToken cancellationToken = default)
        {
            if (token is null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.ConfirmEmailGetAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, token, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //User/Admin
        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="userInfo">userInfo</param>
        /// <returns></returns>
        [HttpPost, Route("password")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateUserPassword([FromBody, CustomizeValidator]UserPasswordToChange userInfo, CancellationToken cancellationToken = default)
        {
            if (userInfo is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdatePasswordAsync(userInfo, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}