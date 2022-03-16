using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Contracts.Requests;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("Auth")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        public AuthorizationController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequest request)
        {
            var authResult = await _authService.RegisterAsync(request);

            if (!authResult.Success)
            {
                return Conflict(authResult.Errors);
            }

            return Ok("Registration was succesful");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            var authResult = await _authService.LoginAsync(request);

            if (!authResult.Success)
            {
                return Conflict(authResult.Errors);
            }

            return Ok("Login was succesful");
        }
    }
}
