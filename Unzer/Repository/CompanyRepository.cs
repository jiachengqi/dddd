using System;
using Microsoft.EntityFrameworkCore;
using Unzer.Data;

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
            return await _context.Companies.Include(c => c.Owners).ToListAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.Include(c => c.Owners)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AddOwnersAsync(int companyId, IEnumerable<Owner> owners)
        {
            var company = await GetCompanyByIdAsync(companyId);
            if (company != null)
            {
                foreach (var owner in owners)
                {
                    company.Owners.Add(owner);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Owner> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            return await _context.Owners
                .Where(o => o.Id == ownerId && o.CompanyId == companyId)
                .FirstOrDefaultAsync();
        }
    }
}

