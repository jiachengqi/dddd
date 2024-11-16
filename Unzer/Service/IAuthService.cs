using System;
using Unzer.Data;

namespace Unzer.Service
{
	public interface IAuthService
	{
        Task<string> LoginAsync(LoginModel login);
    }
}

