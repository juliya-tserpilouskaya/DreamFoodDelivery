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
using System.Threading;

namespace DreamFoodDelivery.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        /// <summary>
        /// Get all dishes by user Id. Id must be verified
        /// </summary>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _basketService.GetAllDishesByUserIdAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? NotFound(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.Data) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Add or updated dish to basket
        /// </summary>
        /// <param name="dishId">dish id</param>
        /// <param name="quantity">dish quantity</param>
        ///// <param name="userId">user id</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "dish added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish id or wrong quantity")]
        [LoggerAttribute]
        public async Task<IActionResult> AddDish([FromBody]string dishId/*, string userId*/, int quantity, CancellationToken cancellationToken = default) 
        {
            if (Guid.TryParse(dishId, out var _) && quantity > 0)
            {
                bool isUser = HttpContext.User.Identity.IsAuthenticated;
                var userIdFromIdentity = HttpContext.User.Claims.Single(_ => _.Type == "id").Value;
                var result = await _basketService.AddUpdateDishAsync(dishId, userIdFromIdentity, quantity, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// Delete dish from basket
        /// </summary>
        /// <param name="dishId">dish id</param>
        /// <returns></returns>
        [HttpDelete, Route("{dishId}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Dish is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string dishId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(dishId, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _basketService.RemoveDishByIdAsync(dishId, HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        ///  Remove all dishes from basket by user Id
        /// </summary>
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes removed")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _basketService.RemoveAllByUserIdAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? NotFound(result.Message) : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}