using System;
using Unzer.Data.DTO;

namespace Unzer.Service
{
	public interface ICompanyService
	{
        Task<IEnumerable<CompanyDTO>> GetCompaniesAsync();
        Task<CompanyDTO> GetCompanyByIdAsync(int id);
        Task<CompanyDTO> CreateCompanyAsync(CompanyDTO companyDto);
        Task UpdateCompanyAsync(int id, CompanyDTO companyDto);
        Task AddOwnersAsync(int companyId, IEnumerable<OwnerDTO> ownerDtos);
        Task AddOwnerAsync(int companyId, OwnerDTO ownerDto);
        Task<bool> CheckSocialSecurityNumberAsync(string ssn);
        Task<OwnerDTO> GetOwnerByIdAsync(int companyId, int ownerId);
    }
}

