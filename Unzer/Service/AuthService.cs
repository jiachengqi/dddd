using Unzer.Data;

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
            string role = login.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

            var token = _jwtTokenGenerator.GenerateToken(login.Username, role);
            return token;
        }
    }
}

