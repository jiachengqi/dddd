using System;
using Unzer.Data;
using Unzer.ExceptionHandling;

namespace Unzer.Service
{
    public class AuthService : IAuthService
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IJwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public string Login(LoginModel login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                throw new BadRequestException("Username and password must be provided.");
            }

            string role = login.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

            var token = _jwtTokenGenerator.GenerateToken(login.Username, role);
            return token;
        }
    }
}

