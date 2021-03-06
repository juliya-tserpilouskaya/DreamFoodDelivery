﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;

namespace DreamFoodDelivery.Web.Controllers
{
    /// <summary>
    /// Users Identity sighin/up and delete actions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Create Identity User & DB User
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns>User identity and db user information with token</returns>
        [AllowAnonymous]
        [HttpPost, Route("registration")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserWithToken))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> RegisterAsync([FromBody]UserRegistration user, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).Join("\n"));
            }
            try
            {
                var result = await _identityService.RegisterAsync(user, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns>User identity and db user information with token</returns>
        [AllowAnonymous]
        [HttpPost, Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserWithToken))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> LoginAsync([FromBody]UserLoginData user, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).Join("\n"));
            }
            try
            {
                var result = await _identityService.LoginAsync(user.Email, user.Password, cancellationToken);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Log out
        /// </summary>
        [HttpPost, Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [LoggerAttribute]
        public IActionResult LogOut()
        {
            
            HttpContext.Session.Clear();
            return Ok();
        }

        /// <summary>
        /// Remove Identity User & DB User
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns>Result information</returns>
        [HttpDelete, Route("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> DeleteAsync([FromBody]UserLoginData user, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).Join("\n"));
            }
            try
            {
                var result = await _identityService.DeleteAsync(user.Email, user.Password, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) 
                     : result.IsSuccess? (IActionResult)Ok(result.IsSuccess) 
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost, Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserWithToken))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> RefreshToken([FromBody]TokenRefresh request, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _identityService.ExchangeRefreshToken(request, cancellationToken);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                    : result.IsSuccess ? (IActionResult)Ok(result.Data)
                    : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }
    }
}