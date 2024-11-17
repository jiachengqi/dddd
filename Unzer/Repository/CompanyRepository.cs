using Microsoft.EntityFrameworkCore;
using Unzer.Data;
using Unzer.ExceptionHandling;

namespace Unzer.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            try
            {
                return await _context.Companies
                    .Include(c => c.Owners)
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("error while retrieving companies.", ex);
            }
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.Owners)
                    .SingleOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    throw new NotFoundException($"company with ID {id} not found.");
                }

                return company;
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("error occurred retrieving company.", ex);
            }
        }

        public async Task CreateCompanyAsync(Company company)
        {
            try
            {
                if (company.Owners == null)
                {
                    company.Owners = new List<Owner>();
                }

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("error occurred creating company.", ex);
            }
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            try
            {
                var existingCompany = await _context.Companies
                    .Include(c => c.Owners)
                    .SingleOrDefaultAsync(c => c.Id == company.Id);

                if (existingCompany == null)
                {
                    throw new NotFoundException($"company with ID {company.Id} not found.");
                }

                existingCompany.Name = company.Name;
                existingCompany.Country = company.Country;
                existingCompany.Email = company.Email;

                var updatedOwners = company.Owners ?? new List<Owner>();

                foreach (var existingOwner in existingCompany.Owners.ToList())
                {
                    if (!updatedOwners.Any(o => o.Id == existingOwner.Id))
                    {
                        _context.Owners.Remove(existingOwner);
                    }
                }

                foreach (var updatedOwner in updatedOwners)
                {
                    var existingOwner = existingCompany.Owners
                        .FirstOrDefault(o => o.Id == updatedOwner.Id);

                    if (existingOwner == null)
                    {
                        if (_context.Entry(updatedOwner).State == EntityState.Detached)
                        {
                            _context.Attach(updatedOwner);
                        }

                        existingCompany.Owners.Add(updatedOwner);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConflictException("Company was updated by another user. Please reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("Error while updating the company.", ex);
            }
        }



        public async Task AddOwnersAsync(int companyId, IEnumerable<Owner> owners)
        {
            try
            {
                var company = await GetCompanyByIdAsync(companyId);
                foreach (var owner in owners)
                {
                    company.Owners.Add(owner);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("error while adding owners to the company.", ex);
            }
        }

        public async Task AddOwnerAsync(int companyId, Owner owner)
        {
            try
            {
                var company = await GetCompanyByIdAsync(companyId);
                company.Owners.Add(owner);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("An error occurred while adding an owner to the company.", ex);
            }
        }

        public async Task<Owner> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            try
            {
                var owner = await _context.Owners
                    .SingleOrDefaultAsync(o => o.Id == ownerId && o.CompanyId == companyId);

                if (owner == null)
                {
                    throw new NotFoundException($"Owner with ID {ownerId} not found in company {companyId}.");
                }

                return owner;
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException("error while retrieving the owner.", ex);
            }
        }
    }
}
