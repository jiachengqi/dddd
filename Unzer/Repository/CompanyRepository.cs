using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unzer.Data;
using Unzer.ExceptionHandling;

namespace Unzer.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyRepository> _logger;

        public CompanyRepository(ApplicationDbContext context, ILogger<CompanyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            try
            {
                return await _context.Companies.Include(c => c.Owners).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error fetching companies.");
                throw new DataAccessException("A database error occurred while fetching the list of companies.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching companies.");
                throw new DataAccessException("An unexpected error occurred while fetching the list of companies.", ex);
            }
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.Owners)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    _logger.LogWarning("Company with ID {Id} was not found.", id);
                    throw new NotFoundException($"Company with ID {id} was not found.");
                }

                return company;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Multiple records found when fetching company by ID: {Id}", id);
                throw new DataAccessException("An unexpected error occurred while fetching the company.", ex);
            }
            catch (DataAccessException)
            {
                throw; // Rethrow already handled DataAccessException
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching company by ID: {Id}", id);
                throw new DataAccessException("An error occurred while fetching the company details.", ex);
            }
        }

        public async Task CreateCompanyAsync(Company company)
        {
            try
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating company: {CompanyName}", company.Name);
                throw new DataAccessException("Failed to create the company. Please check the data and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating company: {CompanyName}", company.Name);
                throw new DataAccessException("An unexpected error occurred while creating the company.", ex);
            }
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            try
            {
                _context.Entry(company).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while updating company: {CompanyId}", company.Id);
                throw new DataAccessException("The company data was modified by another process. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating company: {CompanyId}", company.Id);
                throw new DataAccessException("Failed to update the company. Please check the data and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating company: {CompanyId}", company.Id);
                throw new DataAccessException("An unexpected error occurred while updating the company.", ex);
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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {CompanyId} was not found.", companyId);
                throw; // Rethrow to preserve the original exception
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while adding owners to company: {CompanyId}", companyId);
                throw new DataAccessException("Failed to add owners. Please check the data and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding owners to company: {CompanyId}", companyId);
                throw new DataAccessException("An unexpected error occurred while adding owners.", ex);
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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {CompanyId} was not found.", companyId);
                throw; // Rethrow to preserve the original exception
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while adding owner to company: {CompanyId}", companyId);
                throw new DataAccessException("Failed to add the owner. Please check the data and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding owner to company: {CompanyId}", companyId);
                throw new DataAccessException("An unexpected error occurred while adding the owner.", ex);
            }
        }

        public async Task<Owner> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            try
            {
                var owner = await _context.Owners
                    .FirstOrDefaultAsync(o => o.Id == ownerId && o.CompanyId == companyId);

                if (owner == null)
                {
                    _logger.LogWarning("Owner with ID {OwnerId} not found for company ID {CompanyId}.", ownerId, companyId);
                    throw new NotFoundException($"Owner with ID {ownerId} not found for company ID {companyId}.");
                }

                return owner;
            }
            catch (NotFoundException)
            {
                throw; // Rethrow to preserve the original exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching owner by ID: {OwnerId} for company ID: {CompanyId}", ownerId, companyId);
                throw new DataAccessException("An error occurred while fetching the owner details.", ex);
            }
        }
    }
}
