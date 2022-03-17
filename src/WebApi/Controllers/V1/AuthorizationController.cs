using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Contracts.V1;
using WebApi.Contracts.V1.Requests;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        public AuthorizationController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequest request)
        {
            // There can be only one...
            return StatusCode(500, "This endpoint has been deprecated");

            //var authResult = await _authService.RegisterAsync(request);

            //if (!authResult.Success)
            //{
            //    return Conflict(authResult.Errors);
            //}

            //return Ok("Registration was succesful");
        }

        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            var authResult = await _authService.LoginAsync(request);

            if (!authResult.Success)
            {
                return Conflict(authResult.Errors);
            }

            return Ok(authResult);
        }

        [HttpPost(ApiRoutes.Auth.Logout)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var logoutResult = await _authService.LogoutAsync(request);

            if (!logoutResult.Success)
            {
                return Conflict(logoutResult.Errors);
            }

            return Ok("Logout succesful");
        }
    }
}
