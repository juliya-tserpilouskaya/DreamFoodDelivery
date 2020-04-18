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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _basketService.GetAllDishesByUserIdAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                return result.IsError ? NotFound(result.Message) : (IActionResult)Ok(result);
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish id")]
        public async Task<IActionResult> AddDish([FromBody]string dishId/*, string userId*/, int quantity) 
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            bool isUser = HttpContext.User.Identity.IsAuthenticated;
            var userIdFromIdentity = HttpContext.User.Claims.Single(_ => _.Type == "id").Value;

            var result = await _basketService.AddUpdateDishAsync(dishId, userIdFromIdentity, quantity);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
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
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string dishId)
        {
            if (!Guid.TryParse(dishId, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                var result = await _basketService.RemoveDishByIdAsync(dishId, HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result);
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
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveAllAsync()
        {
            try
            {
                var result = await _basketService.RemoveAllByUserIdAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                return result.IsError ? NotFound(result.Message) : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}