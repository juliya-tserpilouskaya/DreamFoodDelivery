using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices.Orders;
using DreamFoodDelivery.Domain.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DreamFoodDelivery.Web.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>Returns all comments stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no comments in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments were found", typeof(IEnumerable<Comment>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _commentService.GetAllAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Get comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid comment id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment was found", typeof(Comment))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _commentService.GetByIdAsync(id);
                return result == null ? NotFound() : (IActionResult)Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get user comments
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("user/{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid UserId")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment wasn't found")]
        [SwaggerResponse(StatusCodes.Status200OK, "ID users comments were found", typeof(IEnumerable<Comment>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> GetByUserId(string id)
        {
            //if (User.Identity.IsAuthenticated)
            if (string.IsNullOrEmpty(id))
            {
                try
                {
                    var result = await _commentService.GetByUserIdAsync(id);
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
        /// Add comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid comment data")]
        public async Task<IActionResult> Create([FromBody/*, CustomizeValidator*/]Comment comment)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var result = await _commentService.AddAsync(comment);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <param name="comment">Comment</param>
        /// <returns>Comments</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment updated", typeof(Comment))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        public async Task<IActionResult> Update([FromBody]Comment comment)
        {

            if (comment is null /*|| !ModelState.IsValid*/)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _commentService.UpdateAsync(comment);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _) || _commentService.GetById(id) == null /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                var result = await _commentService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete comments
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
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _) || _commentService.GetById(id) == null /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                _commentService.RemoveAllByUserId(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete comments
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments removed")]
        public IActionResult RemoveAll()
        {
            _commentService.RemoveAll();
            return Ok();
        }
    }
}