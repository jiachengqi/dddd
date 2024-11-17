
using Unzer.Data;

namespace Unzer.Repository
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetCompaniesAsync();
        Task<Company> GetCompanyByIdAsync(int id);
        Task CreateCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task AddOwnersAsync(int companyId, IEnumerable<Owner> owners);
        Task AddOwnerAsync(int companyId, Owner owner);
        Task<Owner> GetOwnerByIdAsync(int companyId, int ownerId);
    }
}

