using System;
using Microsoft.AspNetCore.Mvc;
using Unzer.Data;
using Unzer.ExceptionHandling;
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
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var token = await _authService.LoginAsync(login);
                _logger.LogInformation("Successfully authenticated user: {Username}", login.Username);
                return Ok(new { Token = token });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request during login for user: {Username}", login.Username);
                return BadRequest(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error during login for user: {Username}", login.Username);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Username}", login.Username);
                return StatusCode(500, new { Message = "An unexpected error occurred during authentication." });
            }
        }
    }
}

