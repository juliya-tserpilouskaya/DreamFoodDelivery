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
using Swashbuckle.AspNetCore.Annotations;

namespace DreamFoodDelivery.Web.Controllers.Menu
{
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
        /// Add dish
        /// </summary>
        /// <param name="dish"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish data")]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]DishToAdd dish, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _menuService.AddAsync(dish, cancellationToken);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update dish
        /// </summary>
        /// <param name="dish">Dish to update</param>
        /// <returns>Dishes</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish updated", typeof(DishView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
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
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete dish
        /// </summary>
        /// <param name="id">Dish id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Dish is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete dishes
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes removed")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid request")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _menuService.RemoveAllAsync(cancellationToken);
            return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
        }
    }
}