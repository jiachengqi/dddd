using System;
using Unzer.Data;
using Unzer.ExceptionHandling;
using Unzer.Util;

namespace Unzer.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger, IJwtTokenGenerator jwtTokenGenerator)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> LoginAsync(LoginModel login)
        {
            try
            {
                // Simulate user validation
                // In a real application, validate against a user store (e.g., database)
                if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                {
                    _logger.LogWarning("Invalid login attempt with empty username or password.");
                    throw new BadRequestException("Username and password must be provided.");
                }

                // For simulation purposes, accept any username/password
                // Assign role based on username
                string role = login.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

                var token = _jwtTokenGenerator.GenerateToken(login.Username, role);
                _logger.LogInformation("Successfully generated JWT token for user: {Username}", login.Username);
                return token;
            }
            catch (BadRequestException)
            {
                throw; // Rethrow to be handled by the controller
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login for user: {Username}", login.Username);
                throw new ServiceException("An unexpected error occurred during authentication.", ex);
            }
        }
    }
}

