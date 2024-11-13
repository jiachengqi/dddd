using System;
namespace Unzer.Service
{
	public interface ISSNValidationService
	{
        Task<bool> ValidateSsnAsync(string ssn);
    }
}

