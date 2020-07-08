using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.Image;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DreamFoodDelivery.Web.Controllers.Image
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageInterface _imageService;

        public ImageController(IImageInterface imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Check/create directory and write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        //[Authorize(Roles = AppIdentityConstants.ADMIN)]
        [HttpPost]
        [Route("upload")]
        [RequestSizeLimit(NumberСonstants.IMAGE_SIZE)] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public async Task<IActionResult> UploadImage([FromQuery]ImageModel file)
        {
            if (file.DishId is null || file.Image is null || string.IsNullOrEmpty(file.DishId))
            {
                return BadRequest();
            }
            try
            {
                var result = await _imageService.UploadImageAsync(file.Image, file.DishId);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                    : !result.IsSuccess ? StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext))
                    : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Check directory and get image
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("image/{dishId}/{imageName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public IActionResult GetImage(string dishId, string imageName)
        {
            if (string.IsNullOrEmpty(imageName) || string.IsNullOrEmpty(dishId))
            {
                return BadRequest();
            }

            try
            {
                var result = _imageService.GetImage(imageName, dishId);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Check directory and get FileInfo[]
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{dishId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public IActionResult GetImageNamesList(string dishId)
        {
            if (string.IsNullOrEmpty(dishId))
            {
                return BadRequest();
            }

            try
            {
                var result = _imageService.GetImagesInfo(dishId);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                     : result.IsSuccess ? (IActionResult)Ok(result.Data)
                     : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }

        /// <summary>
        /// Remove file from directory by directory and file name
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        //[Authorize(Roles = AppIdentityConstants.ADMIN)]
        [HttpDelete]
        [Route("image/{dishId}/{imageName}")]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustumResult))]
        [LoggerAttribute]
        public IActionResult Delete(string dishId, string imageName)
        {
            if (string.IsNullOrEmpty(imageName) || string.IsNullOrEmpty(dishId))
            {
                return BadRequest();
            }
            try
            {
                var result = _imageService.DeleteImageByName(imageName, dishId);

                return result.IsError ? throw new InvalidOperationException(result.Message)
                    : !result.IsSuccess ? StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext))
                    : StatusCode(StatusCodes.Status206PartialContent, result.Message.CollectProblemDetailsPartialContent(HttpContext));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CustumResult() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message });
            }
        }
    }
}