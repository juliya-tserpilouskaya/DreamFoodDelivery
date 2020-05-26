using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using DreamFoodDelivery.Common;
using System.Threading;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Authorization;

namespace DreamFoodDelivery.Web.Controllers
{
    /// <summary>
    /// Work with menu, allowed for all
    /// </summary>
    [AllowAnonymous]
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
        /// <returns>Returns all dishes in menu</returns>
        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _menuService.GetAllAsync(cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all dishes by request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("request")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAllDishesByRequestAsync([FromBody, CustomizeValidator] RequestParameters request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetAllDishesByRequestAsync(request, cancellationToken);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        

        /// <summary>
        /// Get dishes by name
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="name">Dish name</param>
        /// <returns>Returns name matching dishes</returns>
        [HttpGet, Route("dishes/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByNameAsync(name, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) 
                     : result.IsSuccess ? (IActionResult)Ok(result.Data) 
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dish by cost
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="priceModel">Dish prices</param>
        /// <returns>Returns dishes in prices limits</returns>
        [HttpPost, Route("dishes/price")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> GetByCost([FromBody, CustomizeValidator]DishByCost priceModel, CancellationToken cancellationToken = default)
        {
            if (!(priceModel.LowerPrice >= 0 && priceModel.LowerPrice <= priceModel.UpperPrice && priceModel.UpperPrice >= 0))
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByPriceAsync(priceModel.LowerPrice, priceModel.UpperPrice, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                 : result.IsSuccess ? (IActionResult)Ok(result.Data)
                 : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dishes on sale
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <returns>Returns dishes on sales</returns>
        [HttpGet, Route("sales")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> GetSales(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _menuService.GetSalesAsync(cancellationToken);
                return result == null ? throw new InvalidOperationException(result.Message) 
                     : result.IsSuccess ? (IActionResult)Ok(result.Data) 
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Returns dishes by tag index
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <returns>Returns dishes by tag index</returns>
        [HttpGet, Route("tag/{tagIndex}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> GetByTagIndex(string tagName, CancellationToken cancellationToken = default)
        {
            if (tagName is null)
            {
                return BadRequest();
            }
            try
            {
                var result = await _menuService.GetByTagIndexAsync(tagName, cancellationToken);
                return result == null ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}