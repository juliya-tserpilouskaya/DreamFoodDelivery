﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace DreamFoodDelivery.Web.Controllers
{
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
        /// Add Identity User & Profile to the DB
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody]UserRegistration user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest()
            //}

            try
            {
                var result = await _identityService.RegisterAsync(user.Email, user.Password);

                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody]UserRegistration user)
        {
            try
            {
                var result = await _identityService.LoginAsync(user.Email, user.Password);

                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Log out
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Clear();

            return Ok(await Task.FromResult(Result.Ok()));
        }

        /// <summary>
        /// Remove identity user
        /// </summary>
        /// <param name="user">User data: Email, Password</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromBody]UserRegistration user)
        {
            try
            {
                var result = await _identityService.DeleteAsync(user.Email, user.Password);

                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}