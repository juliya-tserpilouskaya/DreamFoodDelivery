using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation.AspNetCore;
//using NSwag.Annotations;

namespace DreamFoodDelivery.Web.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [AllowAnonymous]
        /// <summary>
        /// Get all tags
        /// </summary>
        /// <returns>Returns all tags stored</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no tags in list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tags were found", typeof(IEnumerable<TagView>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tagService.GetAllAsync();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Create tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tag added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid tag data")]
        public async Task<IActionResult> Create([FromBody, CustomizeValidator]TagToAdd tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _tagService.AddAsync(tag);
            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }

        /// <summary>
        /// Update tag
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>tag</returns>
        [HttpPut, Route("")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tag doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tag updated", typeof(TagView))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
        public async Task<IActionResult> Update([FromBody, CustomizeValidator]TagToUpdate tag)
        {

            if (tag is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _tagService.UpdateAsync(tag);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete tag
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tag doesn't exists")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tag deleted")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
        public async Task<IActionResult> RemoveById(string id)
        {
            if (!Guid.TryParse(id, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
            {
                return BadRequest();
            }
            try
            {
                var result = await _tagService.RemoveByIdAsync(id);
                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete tags
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tags removed")]
        public async Task<IActionResult> RemoveAllAsync()
        {
            await _tagService.RemoveAllAsync();
            return Ok();
        }
    }
}