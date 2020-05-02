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

namespace DreamFoodDelivery.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //Admin
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>Returns all orders stored</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no orders in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders were found", typeof(IEnumerable<OrderView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of orders is empty")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderService.GetAllAsync(cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously get order by order Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid order id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order was found", typeof(OrderView))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Order is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Admin
        /// <summary>
        /// Asynchronously update order status
        /// </summary>
        /// <param name="orderStatus">New personal discount</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut, Route("status")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order updated", typeof(OrderView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously create new order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order added", typeof(OrderView))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid order data")]
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
                var result = await _orderService.AddAsync(order, HttpContext.User.Claims.Single(_ => _.Type == "id").Value, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns>order</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order updated", typeof(OrderToUpdate))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
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
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get user orders
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("user/{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid UserId")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Orders wasn't found")]
        [SwaggerResponse(StatusCodes.Status200OK, "ID users orders were found", typeof(IEnumerable<OrderView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetByUserId(string id, CancellationToken cancellationToken = default)
        {
            //if (User.Identity.IsAuthenticated)
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var result = await _orderService.GetByUserIdAsync(id, cancellationToken);
                    return result == null ? NotFound() : (IActionResult)Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete order
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Order is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) //проверить везде
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete orders
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders removed")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of orders is empty")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid request")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderService.RemoveAllAsync(cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete orders by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("user/{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Orders doesn't exists")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of orders is empty")]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders deleted")]
        public async Task<IActionResult> RemoveAllByUserId(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.RemoveAllByUserIdAsync(id, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}