using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDbService<UserDomainClass> _dbContext;

        public HomeController(IDbService<UserDomainClass> dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello World!" });
        }

        [HttpGet("Home")]
        public async Task<IActionResult> Home()
        {
            return Ok(await _dbContext.GetAll());
        }

        [HttpGet("InvalidProtocol")]
        public IActionResult InvalidProtocol()
        {
            return BadRequest("This API does not listen on HTTP. Please use HTTPS exclusively");
        }
    }
}
