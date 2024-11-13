using System;
using Microsoft.AspNetCore.Mvc;
using Unzer.Data;
using Unzer.Util;

namespace Unzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Validate user credentials
            // For simulation, accept any username/password
            // Assign role based on username
            string role = login.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

            var token = JwtTokenGenerator.GenerateToken(login.Username, role, _configuration);
            return Ok(new { Token = token });
        }
    }
}

