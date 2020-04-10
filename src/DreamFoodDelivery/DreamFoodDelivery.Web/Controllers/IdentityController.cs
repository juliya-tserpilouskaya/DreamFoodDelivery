using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DreamFoodDelivery.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Register([FromBody]UserToRegister userToRegister)
        {
            var result = await _identityService.RegisterAsync(userToRegister.Email, userToRegister.Password);

            return result.IsError ? BadRequest(result.Message) : (IActionResult)Ok(result.Data);
        }
    }
}