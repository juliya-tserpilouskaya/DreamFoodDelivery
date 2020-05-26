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
using NSwag.Annotations;

namespace DreamFoodDelivery.Web.Controllers
{
    /// <summary>
    /// Work with orders
    /// </summary>
    //[Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>Returns all orders stored</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("all")]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status200OK, typeof(IEnumerable<OrderView>))]
        //[ProducesResponseType(typeof(IEnumerable<OrderView>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderService.GetAllAsync(cancellationToken);
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
        /// Get order by order Id
        /// </summary>
        /// <param name="id">Order id</param>
        /// <returns>Returns ID matching order</returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderView))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.GetByIdAsync(id, cancellationToken);
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
        /// Asynchronously update order status
        /// </summary>
        /// <param name="orderStatus">New order status</param>
        /// <returns>Returns order information</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut, Route("status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateStatus([FromBody, CustomizeValidator]OrderToStatusUpdate orderStatus, CancellationToken cancellationToken = default)
        {
            if (orderStatus is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(orderStatus, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="order">New order to add</param>
        /// <returns>Order info after adding</returns>
        [HttpPost, Route("")]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status200OK, typeof(IEnumerable<OrderView>))]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status400BadRequest, typeof(ProblemDetails))]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status403Forbidden, typeof(ProblemDetails), Description = "Please, confirm your email")]
        //[NSwag.Annotations.SwaggerResponse(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]OrderToAdd order, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var isEmailConf = await _userService.IsEmailConfirmedAsync(HttpContext.User.Claims.Single(_ => _.Type == "id").Value);
                if (isEmailConf.IsSuccess)
                {
                    var result = await _orderService.AddAsync(order, HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                    return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.Data);
                }
                else
                {
                    //return StatusCode(StatusCodes.Status500InternalServerError, "Confirm your email");
                    //BadRequest("Confirm your email");
                    //Unauthorized("Confirm your email");
                    //StatusCode(403, "Confirm your email");
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="order">Order to update</param>
        /// <returns>Order info after updatting</returns>
        [HttpPut, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]OrderToUpdate order, CancellationToken cancellationToken = default)
        {
            if (order is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.UpdateAsync(order, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get user orders for administration
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Returns users orders for administration</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetByUserIdAdmin(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.GetByUserIdAdminAsync(id, cancellationToken);
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
        /// Get users orders for users actions
        /// </summary>
        /// <returns>Returns users orders for users actions</returns>
        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetByUserId(CancellationToken cancellationToken = default)
        {
            try
            {
                string userIdFromIdentity = HttpContext.User.Claims.Single(_ => _.Type == "id").Value;
                var result = await _orderService.GetByUserIdAsync(userIdFromIdentity, cancellationToken);
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
        /// Delete order
        /// </summary>
        /// <param name="id">Order id to delete</param>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                var result = await _orderService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
                     : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete orders
        /// </summary>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderService.RemoveAllAsync(cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
                     : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete orders by user id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllByUserId(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.RemoveAllByUserIdAsync(id, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess)
                     : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all statuses from DB
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("statuses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderStatus>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult>GetStatuses(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _orderService.GetStatuses(cancellationToken);
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
        /// Asynchronously return all order in status
        /// </summary>
        /// <param name="statusName">Status index</param>
        /// <returns>Result information</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("status/{statusName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetOrdersInStatus(string statusName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(statusName))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.GetOrdersInStatus(statusName, cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : NotFound(result.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}