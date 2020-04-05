using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DreamFoodDelivery.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>Returns all orders stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no orders in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Orders were found", typeof(IEnumerable<OrderView>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Get order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid order id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "order doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "order was found", typeof(OrderView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
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

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "order added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid order data")]
        public async Task<IActionResult> Create([FromBody/*, CustomizeValidator*/]OrderToAdd order)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var result = await _orderService.AddAsync(order);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
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
        public async Task<IActionResult> Update([FromBody]OrderToUpdate order)
        {
            //temp:
            Guid id = Guid.NewGuid(); //Guid.Parse(Id);
            if (order is null /*|| !ModelState.IsValid*/)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.UpdateAsync(order, id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        public async Task<IActionResult> RemoveById(string id)
        {
            if (!Guid.TryParse(id, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
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
            _orderService.RemoveAllAsync();
            return Ok();
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
        public async Task<IActionResult> GetByUserId(string id)
        {
            //if (User.Identity.IsAuthenticated)
            if (string.IsNullOrEmpty(id))
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
        /// Delete orders
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
            if (!Guid.TryParse(id, out var _) /*|| _commentService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                _orderService.RemoveAllByUserIdAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }     
    }
}