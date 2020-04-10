//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using DreamFoodDelivery.Domain.Logic.InterfaceServices;
//using DreamFoodDelivery.Domain.DTO;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.Annotations;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

//namespace DreamFoodDelivery.Web.Controllers
//{
//    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        public UserController(IUserService userService)
//        {
//            _userService = userService;
//        }

//        /// <summary>
//        /// Get all users
//        /// </summary>
//        /// <returns>Returns all users stored</returns>
//        [HttpGet, Route("")]
//        [SwaggerResponse(StatusCodes.Status404NotFound, "There are no users in list")]
//        [SwaggerResponse(StatusCodes.Status200OK, "Users were found", typeof(IEnumerable<UserDTO>))]
//        public async Task<IActionResult> GetAll()
//        {
//            var result = await _userService.GetAllAsync();
//            return result == null ? NotFound() : (IActionResult)Ok(result);
//        }

//        /// <summary>
//        /// Create new account
//        /// </summary>
//        /// <param name="user">user</param>
//        /// <returns></returns>
//        [HttpPost, Route("")]
//        [SwaggerResponse(StatusCodes.Status200OK, "User added")]
//        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid user data")]
//        public async Task<IActionResult> CreateAsync([FromBody] UserDTO user)
//        {
//            if (user is null /*|| ModelState.IsValid*/) 
//            {
//                return BadRequest(ModelState);
//            }
//            var result = await _userService.CreateAccountAsync(user); 
//            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
//        }

//        /// <summary>
//        /// Get user account by Id
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet, Route("{id}")]
//        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid user id")]
//        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
//        [SwaggerResponse(StatusCodes.Status200OK, "User was found", typeof(UserDTO))]
//        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
//        public async Task<IActionResult> GetById(string id)
//        {
//            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _))
//            {
//                return BadRequest();
//            }
//            try
//            {
//                var result = await _userService.GetByIdAsync(id);
//                return result == null ? NotFound() : (IActionResult)Ok(result);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }


//        /// <summary>
//        /// Update the user account
//        /// </summary>
//        /// <param name="user">user</param>
//        /// <returns></returns>
//        [HttpPut, Route("")]
//        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paramater format")]
//        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
//        [SwaggerResponse(StatusCodes.Status200OK, "User updated", typeof(UserDTO))]
//        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something wrong")]
//        public async Task<IActionResult> Update([FromBody]UserDTO user)
//        {

//            if (user is null /*|| !ModelState.IsValid*/)
//            {
//                return BadRequest(ModelState);
//            }
//            try
//            {
//                var result = await _userService.UpdateAsync(user);
//                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }

//        /// <summary>
//        /// Remove user
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpDelete, Route("{id}")]
//        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ivalid ID")]
//        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't exists")]
//        [SwaggerResponse(StatusCodes.Status200OK, "User deleted")]
//        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something goes wrong")]
//        public async Task<IActionResult> RemoveById(string id)
//        {
//            if (!Guid.TryParse(id, out var _) /*|| _orderService.GetById(id) == null*/ /*|| _commentService.GetById(id).UserId != UserId*/)
//            {
//                return BadRequest();
//            }
//            try
//            {
//                var result = await _userService.RemoveByIdAsync(id);
//                return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.IsSuccess);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }
//    }
//}