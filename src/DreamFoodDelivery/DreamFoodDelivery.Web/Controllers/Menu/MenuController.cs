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
using DreamFoodDelivery.Common;
using System.Threading;

namespace DreamFoodDelivery.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _menuService.GetAllAsync(cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "Dish is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByIdAsync(id, cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish by name
        /// </summary>
        /// <param name="name">Dish name</param>
        /// <returns>Dishes</returns>
        [HttpGet, Route("dishes/{name}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid parameter format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dishes are not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes are found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByNameAsync(name, cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> GetByCategory(string categoryString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(categoryString))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByCategoryAsync(categoryString, cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetByCost(string priceString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(priceString))
            {
                return BadRequest();
            }
            try
            {
                string[] priceSplited = priceString.Split('_');
                double lowerPrice = double.Parse(priceSplited[0]);
                double upperPrice = double.Parse(priceSplited[1]);
                if (lowerPrice > 0 && lowerPrice < upperPrice && upperPrice > 0)
                {
                    var result = await _menuService.GetByPriceAsync(lowerPrice, upperPrice, cancellationToken);
                    return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); //check number
            } 
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); //check number
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something went wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetSales(CancellationToken cancellationToken = default)
        {
            var result = await _menuService.GetSalesAsync(cancellationToken);
            return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
        }

        /// <summary>
        /// Returns dishes by tag index
        /// </summary>
        /// <returns>Returns dishes by tag index</returns>
        [HttpGet, Route("tag/{tagIndex}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no dishes in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dishes were found", typeof(IEnumerable<DishView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of dishes is empty")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetByTagIndex(int tagIndex, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _menuService.GetByTagIndexAsync(tagIndex, cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}