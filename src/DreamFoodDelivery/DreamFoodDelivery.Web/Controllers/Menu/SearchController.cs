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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DreamFoodDelivery.Web.Controllers.Menu
{
    /// <summary>
    /// Menu search
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _service;
        public SearchController(ISearchService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all tags from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("tags")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TagView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult>GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetAllTagsAsync(cancellationToken);

                return result.IsError ? throw new InvalidOperationException(result.Message) : Ok(result.Data);
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
                var result = await _service.GetAllDishesByRequestAsync(request, cancellationToken);

                return result.IsError ? throw new InvalidOperationException(result.Message)
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