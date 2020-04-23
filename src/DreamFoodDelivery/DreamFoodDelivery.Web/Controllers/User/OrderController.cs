using System;
using System.Collections.Generic;
using System.Linq;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no orders in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders were found", typeof(IEnumerable<OrderView>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _orderService.GetAllAsync();
                return result == null ? NotFound() : (IActionResult)Ok(result);
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
        [SwaggerResponse(StatusCodes.Status404NotFound, "order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "order was found", typeof(OrderView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.GetByIdAsync(id);
                return result == null ? NotFound() : (IActionResult)Ok(result);
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
        [HttpPut, Route("status")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> UpdateStatus([FromBody, CustomizeValidator]OrderToStatusUpdate orderStatus)
        {
            if (orderStatus is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(orderStatus);
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
        [SwaggerResponse(StatusCodes.Status200OK, "order added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid order data")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]OrderToAdd order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.AddAsync(order);
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
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]OrderToUpdate order)
        {
            if (order is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.UpdateAsync(order);
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
        [SwaggerResponse(StatusCodes.Status404NotFound, "orders wasn't found")]
        [SwaggerResponse(StatusCodes.Status200OK, "ID users orders were found", typeof(IEnumerable<OrderView>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetByUserId(string id)
        {
            //if (User.Identity.IsAuthenticated)
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var result = await _orderService.GetByUserIdAsync(id);
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
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "order deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) //проверить везде
            {
                return BadRequest();
            }
            try
            {
                var result = await _orderService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
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
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "orders removed")]
        public IActionResult RemoveAll()
        {
            try
            {
                _orderService.RemoveAll(); //use Result
                return Ok();
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
        [HttpDelete, Route("user/{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comments doesn't exists")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments deleted")]
        public IActionResult RemoveAllByUserId(string id)
        {
            if (!Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                _orderService.RemoveAllByUserId(id); //use Result
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}