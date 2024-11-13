using System;
namespace Unzer.Service
{
    public class SocialSecurityService : ISocialSecurityService
    {
        public async Task<bool> CheckSocialSecurityNumberAsync(string ssn)
        {
            // Simulate a backend check by randomly returning valid/invalid
            await Task.Delay(100); // Simulate network delay
            var random = new Random();
            return random.Next(2) == 0;
        }
    }
}

