using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DreamFoodDelivery.Web.Controllers.Menu
{
    /// <summary>
    /// Work with dishes database, for Admins only
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public DishController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Add new dish with tags
        /// </summary>
        /// <param name="dish">New dish to add</param>
        /// <returns>Dish info after adding</returns>
        [HttpPost, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]DishToAdd dish, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _menuService.AddAsync(dish, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : BadRequest(result.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update dish with tags
        /// </summary>
        /// <param name="dish">Dish to update</param>
        /// <returns>Dish info after updating</returns>
        [HttpPut, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]DishToUpdate dish, CancellationToken cancellationToken = default)
        {
            if (dish is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _menuService.UpdateAsync(dish, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : BadRequest(result.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete dish
        /// </summary>
        /// <param name="id">Dish id to delete</param>
        /// <returns>Result information</returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.RemoveByIdAsync(id, cancellationToken);
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
        /// Delete dishes
        /// </summary>
        /// <returns>Result information</returns>
        [HttpDelete, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _menuService.RemoveAllAsync(cancellationToken);
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