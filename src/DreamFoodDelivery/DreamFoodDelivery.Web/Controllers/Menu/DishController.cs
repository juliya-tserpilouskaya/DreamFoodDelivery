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
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        /// <summary>
        /// Add dish to basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "dish added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish id")]
        public async Task<IActionResult> AddDish([FromBody]string id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var result = await _dishService.AddDishAsync(id);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Delete dish from basket
        /// </summary>
        /// <param name="id">dish id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (!Guid.TryParse(id, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                var result = await _dishService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all dishes
        /// </summary>
        /// <returns>Returns all dishes stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no dishs in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "dishs were found", typeof(IEnumerable<Dish>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dishService.GetAllAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Get dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish was found", typeof(Dish))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _dishService.GetByIdAsync(id);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}