using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello World!" });
        }
    }
}
