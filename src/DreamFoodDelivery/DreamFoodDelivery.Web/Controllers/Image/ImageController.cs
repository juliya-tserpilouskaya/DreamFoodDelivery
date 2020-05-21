using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Common.Сonstants;
using DreamFoodDelivery.Domain.Image;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("upload")]
        [RequestSizeLimit(Number_Сonstants.IMAGE_SIZE)] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [LoggerAttribute]
        public async Task<IActionResult> UploadImage([FromQuery]ImageModel file)
        {
            if (file is null || file.Image.Length == 0 || string.IsNullOrEmpty(file.DishId))
            {
                return BadRequest();
            }
            try
            {
                var result = await _imageService.UploadImageAsync(file.Image, file.DishId);

                return result.IsError ? throw new InvalidOperationException(result.Message) : Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                return result.IsError
                    ? throw new InvalidOperationException(result.Message)
                    : !result.IsSuccess
                    ? NotFound(result.Message)
                    : (IActionResult)Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                return result.IsError
                    ? throw new InvalidOperationException(result.Message)
                    : !result.IsSuccess
                    ? NotFound(result.Message)
                    : (IActionResult)Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Remove file from directory by directory and file name
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        //[Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("image/{dishId}/{imageName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                return result.IsError
                    ? throw new InvalidOperationException(result.Message)
                    : !result.IsSuccess
                    ? NotFound(result.Message)
                    : (IActionResult)NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}