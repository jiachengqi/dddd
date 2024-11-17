namespace Unzer.Service
{
	public interface IJwtTokenGenerator
	{
        string GenerateToken(string username, string role);
    }
}

