using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Unzer.Data;
using Unzer.Data.DTO;
using Unzer.ExceptionHandling;
using Unzer.Repository;
using Microsoft.AspNetCore.Http;

namespace Unzer.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ISSNValidationService _ssnService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            ICompanyRepository companyRepository,
            IMapper mapper,
            ISSNValidationService ssnService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _ssnService = ssnService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyDTO>> GetCompaniesAsync()
        {
            try
            {
                var companies = await _companyRepository.GetCompaniesAsync();
                var companiesDto = _mapper.Map<IEnumerable<CompanyDTO>>(companies);

                bool canReadSSN = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

                if (!canReadSSN)
                {
                    foreach (var company in companiesDto)
                    {
                        if (company.Owners != null)
                        {
                            foreach (var owner in company.Owners)
                            {
                                owner.SocialSecurityNumber = null;
                            }
                        }
                    }
                }

                return companiesDto;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while fetching companies.");
                throw new ServiceException("Mapping error occurred while fetching companies.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching companies.");
                throw new ServiceException("Error fetching companies from the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching companies.");
                throw new ServiceException("An unexpected error occurred while fetching companies.", ex);
            }
        }

        public async Task<CompanyDTO> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return _mapper.Map<CompanyDTO>(company);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {Id} was not found.", id);
                throw; // Rethrow to preserve the original exception
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while fetching company by ID: {Id}", id);
                throw new ServiceException("Mapping error occurred while fetching the company by ID.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching company with ID: {Id}", id);
                throw new ServiceException("Error retrieving the company from the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching company with ID: {Id}", id);
                throw new ServiceException("An unexpected error occurred while retrieving the company.", ex);
            }
        }

        public async Task<CompanyDTO> CreateCompanyAsync(CompanyDTO companyDto)
        {
            try
            {
                var company = _mapper.Map<Company>(companyDto);
                await _companyRepository.CreateCompanyAsync(company);
                _logger.LogInformation("New company created: {CompanyName}", companyDto.Name);
                return _mapper.Map<CompanyDTO>(company);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while creating company: {CompanyName}", companyDto.Name);
                throw new ServiceException("Mapping error occurred while creating the company.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating company: {CompanyName}", companyDto.Name);
                throw new ServiceException("Error creating a new company record in the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating company: {CompanyName}", companyDto.Name);
                throw new ServiceException("An unexpected error occurred while creating the company.", ex);
            }
        }

        public async Task UpdateCompanyAsync(int id, CompanyDTO companyDto)
        {
            try
            {
                if (id != companyDto.Id)
                {
                    _logger.LogWarning("Company ID mismatch: {Id} != {CompanyId}", id, companyDto.Id);
                    throw new BadRequestException("Company ID mismatch.");
                }

                var company = _mapper.Map<Company>(companyDto);
                await _companyRepository.UpdateCompanyAsync(company);
                _logger.LogInformation("Company with ID: {Id} updated", id);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Company ID mismatch: {Id} != {CompanyId}", id, companyDto.Id);
                throw; // Rethrow to preserve the original exception
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while updating company with ID: {Id}", id);
                throw new ServiceException("Mapping error occurred while updating the company.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating company with ID: {Id}", id);
                throw new ServiceException("Error updating the company in the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating company with ID: {Id}", id);
                throw new ServiceException("An unexpected error occurred while updating the company.", ex);
            }
        }

        public async Task AddOwnersAsync(int companyId, IEnumerable<OwnerDTO> ownerDtos)
        {
            try
            {
                var owners = _mapper.Map<IEnumerable<Owner>>(ownerDtos);
                await _companyRepository.AddOwnersAsync(companyId, owners);
                _logger.LogInformation("Added owners to company with ID: {CompanyId}", companyId);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while adding owners to company with ID: {CompanyId}", companyId);
                throw new ServiceException("Mapping error occurred while adding owners.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding owners to company with ID: {CompanyId}", companyId);
                throw new ServiceException("Error adding owners to the company.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding owners to company with ID: {CompanyId}", companyId);
                throw new ServiceException("An unexpected error occurred while adding owners.", ex);
            }
        }

        public async Task AddOwnerAsync(int companyId, OwnerDTO ownerDto)
        {
            try
            {
                var owner = _mapper.Map<Owner>(ownerDto);
                await _companyRepository.AddOwnerAsync(companyId, owner);
                _logger.LogInformation("Added owner to company with ID: {CompanyId}", companyId);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while adding an owner to company with ID: {CompanyId}", companyId);
                throw new ServiceException("Mapping error occurred while adding an owner.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding an owner to company with ID: {CompanyId}", companyId);
                throw new ServiceException("Error adding an owner to the company.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding an owner to company with ID: {CompanyId}", companyId);
                throw new ServiceException("An unexpected error occurred while adding the owner.", ex);
            }
        }

        public async Task<bool> CheckSocialSecurityNumberAsync(string ssn)
        {
            try
            {
                return await _ssnService.ValidateSsnAsync(ssn);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "External service error during SSN validation.");
                throw new ServiceException("Error validating the Social Security Number with the external service.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while validating SSN: {SSN}", ssn);
                throw new ServiceException("An unexpected error occurred while validating the Social Security Number.", ex);
            }
        }

        public async Task<OwnerDTO> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            try
            {
                var owner = await _companyRepository.GetOwnerByIdAsync(companyId, ownerId);
                return _mapper.Map<OwnerDTO>(owner);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Owner with ID {OwnerId} not found for company ID {CompanyId}.", ownerId, companyId);
                throw; // Rethrow to preserve the original exception
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping error occurred while fetching owner by ID: {OwnerId} for company ID: {CompanyId}", ownerId, companyId);
                throw new ServiceException("Mapping error occurred while fetching the owner by ID.", ex);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching owner with ID: {OwnerId} for company ID: {CompanyId}", ownerId, companyId);
                throw new ServiceException("Error retrieving the owner from the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching owner with ID: {OwnerId} for company ID: {CompanyId}", ownerId, companyId);
                throw new ServiceException("An unexpected error occurred while retrieving the owner.", ex);
            }
        }
    }
}
