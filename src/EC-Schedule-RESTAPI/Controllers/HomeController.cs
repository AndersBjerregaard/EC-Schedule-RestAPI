using EC_Schedule_RESTAPI.Data;
using EC_Schedule_RESTAPI.Domain;
using EC_Schedule_RESTAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC_Schedule_RESTAPI.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Test")]
        public IActionResult GetAllTest()
        {
            var objects = _dbContext.TestObjects;

            return Ok(objects.ToArray());
        }

        [HttpPost("Test")]
        public async Task<IActionResult> TestPost([FromBody] DomainTestObject objectParameter)
        {
            _dbContext.TestObjects.Add(objectParameter);

            await _dbContext.SaveChangesAsync();

            return Ok(objectParameter);
        }
    }
}
