using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation.AspNetCore;

namespace DreamFoodDelivery.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishMenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public DishMenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Returns menu (all dishes)
        /// </summary>
        /// <returns>Returns all dishes stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no dishes in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes were found", typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _menuService.GetAllAsync();
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish was found", typeof(DishView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByIdAsync(id);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Dishes</returns>
        [HttpGet, Route("dishes/name/{name}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid parameter format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dishes are not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes are found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        public async Task<IActionResult> GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByNameAsync(name);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish by category
        /// </summary>
        /// <param name="categoryString"></param>
        /// <returns>Dishes</returns>
        [HttpGet, Route("dishes/category/{categoryString}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid parameter format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dishes are not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes are found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        public async Task<IActionResult> GetByCategory(string categoryString)
        {
            if (string.IsNullOrEmpty(categoryString))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByCategoryAsync(categoryString);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish by cost
        /// </summary>
        /// <param name="priceString">Dish price</param>
        /// <returns>Dishes</returns>
        [HttpGet, Route("dishes/price/{priceString}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid parameter format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dishes are not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes are found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        public async Task<IActionResult> GetByCost(string priceString)
        {
            if (string.IsNullOrEmpty(priceString))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByPriceAsync(priceString);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dishes on sale
        /// </summary>
        /// <returns>Dishes</returns>
        [HttpGet, Route("sales")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid parameter format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dishes are not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes are found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        public async Task<IActionResult> GetSales()
        {
            var result = await _menuService.GetSalesAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Add dish
        /// </summary>
        /// <param name="dish"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid dish data")]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]DishToAdd dish)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _menuService.AddAsync(dish);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update dish
        /// </summary>
        /// <param name="dish">dish</param>
        /// <returns>Dishes</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish updated", typeof(DishView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]DishToUpdate dish)
        {
            if (dish is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _menuService.UpdateAsync(dish);
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
        /// <param name="id">dish id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dish deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
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
        public async Task<IActionResult> RemoveAllAsync()
        {
            await _menuService.RemoveAllAsync();
            return Ok();
        }
    }
}