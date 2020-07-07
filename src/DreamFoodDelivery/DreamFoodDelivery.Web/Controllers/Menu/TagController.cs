using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation.AspNetCore;
using DreamFoodDelivery.Common;
using System.Threading;
using DreamFoodDelivery.Domain.View;

namespace DreamFoodDelivery.Web.Controllers
{
    /// <summary>
    /// Work with tags
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppIdentityConstants.ADMIN)]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Get all tags from DB
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TagView>))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _tagService.GetAllTagsAsync(cancellationToken);
                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Create tag
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [ObsoleteAttribute]
        [HttpPost, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LoggerAttribute]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]TagToAdd tag, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _tagService.AddAsync(tag, cancellationToken);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update tag
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>tag</returns>
        [ObsoleteAttribute]
        [HttpPut, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]TagToUpdate tag, CancellationToken cancellationToken = default)
        {

            if (tag is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _tagService.UpdateAsync(tag, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete tag
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> RemoveById(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var _))
            {
                return BadRequest();
            }
            try
            {
                var result = await _tagService.RemoveByIdAsync(id, cancellationToken);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete tags
        /// !!! Obsolete controller. If necessary, review their return data types and status codes!!!
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [LoggerAttribute]
        [ObsoleteAttribute]
        public async Task<IActionResult> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            await _tagService.RemoveAllAsync(cancellationToken);
            return Ok();
        }
    }
}