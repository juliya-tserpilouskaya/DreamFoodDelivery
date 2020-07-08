using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DreamFoodDelivery.Web.Controllers.Menu
{
    /// <summary>
    /// Work with dishes database, for Admins only
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppIdentityConstants.ADMIN)]
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        /// <summary>
        /// Add new dish with tags
        /// </summary>
        /// <param name="dish">New dish to add</param>
        /// <returns>Dish info after adding</returns>
        [HttpPost, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]DishToAdd dish, CancellationToken cancellationToken = default)
        {
            
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _dishService.AddAsync(dish, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     //: BadRequest(new ProblemDetails() {
                     //    Detail = result.Message });
                     : StatusCode(StatusCodes.Status400BadRequest, result.Message);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get dish by id
        /// </summary>
        /// <param name="id">Dish id</param>
        /// <returns>Returns ID matching dish</returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishView))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _dishService.GetByIdAsync(id, cancellationToken);
                return result == null ? throw new InvalidOperationException(result.Message)
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
        /// Update dish with tags
        /// </summary>
        /// <param name="dish">Dish to update</param>
        /// <returns>Dish info after updating</returns>
        [HttpPut, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishView))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]DishToUpdate dish, CancellationToken cancellationToken = default)
        {
            if (dish is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _dishService.UpdateAsync(dish, cancellationToken);
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
        /// Delete dish
        /// </summary>
        /// <param name="id">Dish id to delete</param>
        /// <returns>Result information</returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _dishService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) 
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) 
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete dishes
        /// </summary>
        /// <returns>Result information</returns>
        [HttpDelete, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _dishService.RemoveAllAsync(cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
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