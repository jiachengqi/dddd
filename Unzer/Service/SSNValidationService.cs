using System;
using Microsoft.EntityFrameworkCore;
using Unzer.Data;
using Unzer.ExceptionHandling;

namespace Unzer.Service
{
	public class SSNValidationService : ISSNValidationService
	{
        private readonly Random _random = new Random();

        public async Task<bool> ValidateSsnAsync(string ssn)
        {
            await Task.Delay(100);

            try
            {
                if (string.IsNullOrWhiteSpace(ssn))
                {
                    throw new BadRequestException("SSN cannot be empty.");
                }

                return _random.Next(0, 2) == 0;
            }
            catch (ExternalServiceException ex)
            {
                throw new ExternalServiceException("error validating SSN", ex);
            }
            
        }
    }
}

