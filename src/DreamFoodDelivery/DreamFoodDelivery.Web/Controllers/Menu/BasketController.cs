using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DreamFoodDelivery.Web.Controllers
{
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
        /// Get all baskets
        /// </summary>
        /// <returns>Returns all baskets stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no baskets in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Baskets were found", typeof(IEnumerable<Basket>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _basketService.GetAllAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Create basket
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "basket added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid basket data")]
        public async Task<IActionResult> Create([FromBody/*, CustomizeValidator*/]Basket basket)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var result = await _basketService.AddAsync(basket);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update basket
        /// </summary>
        /// <param name="basket">basket</param>
        /// <returns>basket</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "basket doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "basket updated", typeof(Basket))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        public async Task<IActionResult> Update([FromBody]Basket basket)
        {

            if (basket is null /*|| !ModelState.IsValid*/)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _basketService.UpdateAsync(basket);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete basket
        /// </summary>
        /// <param name="id">basket id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "basket doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "basket deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (!Guid.TryParse(id, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                var result = await _basketService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}