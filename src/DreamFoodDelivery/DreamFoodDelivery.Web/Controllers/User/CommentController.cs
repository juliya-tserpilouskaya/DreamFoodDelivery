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
        /// <param name="cancellationToken"></param>
        /// <returns>Returns all comments stored</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no comments in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments were found", typeof(IEnumerable<CommentView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of comments is empty")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _commentService.GetAllAsync(cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get comment by comment id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid comment id")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment was found", typeof(CommentView))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Comment is missing")]
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
                var result = await _commentService.GetByIdAsync(id, cancellationToken);
                return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
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
        [SwaggerResponse(StatusCodes.Status200OK, "ID users comments were found", typeof(IEnumerable<CommentView>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of comments is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> GetByUserId(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) && Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    var result = await _commentService.GetByUserIdAsync(id, cancellationToken);
                    return result == null ? NotFound() : result.IsSuccess ? (IActionResult)Ok(result) : NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        /// <summary>
        /// Asynchronously add new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment added", typeof(CommentView))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid comment data")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]CommentToAdd comment, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _commentService.AddAsync(comment, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <param name="comment">Comment to update</param>
        /// <returns>Comments</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment updated", typeof(CommentView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]CommentToUpdate comment, CancellationToken cancellationToken = default)
        {

            if (comment is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _commentService.UpdateAsync(comment, cancellationToken);
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
        /// <param name="id">Comment id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comment doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Comment is missing")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) //check it
            {
                return BadRequest();
            }
            try
            {
                var result = await _commentService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete comments by user id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("user/{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comments doesn't exists")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments deleted")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of comments is empty")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAllByUserId(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _commentService.RemoveAllByUserIdAsync(id, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
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
        [Authorize(Roles = "Admin")]
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid request")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments removed")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "List of comments is empty")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        [LoggerAttribute]
        public async Task<IActionResult> RemoveAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _commentService.RemoveAllAsync(cancellationToken);
                return result.IsError ? BadRequest(result.Message) : result.IsSuccess ? (IActionResult)Ok(result.IsSuccess) : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}