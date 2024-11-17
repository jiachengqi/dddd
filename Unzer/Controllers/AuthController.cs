using Microsoft.AspNetCore.Mvc;
using Unzer.Data;
using Unzer.Service;

namespace Unzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var token = _authService.Login(login);
            _logger.LogInformation("authenticated user: {Username}", login.Username);
            return Ok(new { Token = token });
        }
    }
}

