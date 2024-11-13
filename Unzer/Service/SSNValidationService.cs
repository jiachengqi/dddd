using System;
namespace Unzer.Service
{
	public class SSNValidationService : ISSNValidationService
	{
        private readonly Random _random = new Random();

        public async Task<bool> ValidateSsnAsync(string ssn)
        {
            await Task.Delay(100); // Simulate network delay
            // Simulate validation by randomly returning true or false
            return _random.Next(0, 2) == 0;
        }
    }
}

