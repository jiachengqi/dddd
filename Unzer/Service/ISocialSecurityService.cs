using System;
namespace Unzer.Service
{
    public interface ISocialSecurityService
    {
        Task<bool> CheckSocialSecurityNumberAsync(string ssn);
    }
}

