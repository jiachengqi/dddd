using System;
using AutoMapper;
using Unzer.Data;
using Unzer.Data.DTO;
using Unzer.Repository;

namespace Unzer.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ISSNValidationService _ssnService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyService(
            ICompanyRepository companyRepository,
            IMapper mapper,
            ISSNValidationService ssnService,
            IHttpContextAccessor httpContextAccessor)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _ssnService = ssnService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CompanyDTO>> GetCompaniesAsync()
        {
            var companies = await _companyRepository.GetCompaniesAsync();
            var companiesDto = _mapper.Map<IEnumerable<CompanyDTO>>(companies);

            // Check user role
            bool canReadSSN = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (!canReadSSN)
            {
                // Remove SSN from owners
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

        public async Task<CompanyDTO> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);
            if (company == null) return null;
            return _mapper.Map<CompanyDTO>(company);
        }

        public async Task<CompanyDTO> CreateCompanyAsync(CompanyDTO companyDto)
        {
            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.CreateCompanyAsync(company);
            return _mapper.Map<CompanyDTO>(company);
        }

        public async Task UpdateCompanyAsync(int id, CompanyDTO companyDto)
        {
            if (id != companyDto.Id)
            {
                throw new KeyNotFoundException("Company ID mismatch.");
            }

            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.UpdateCompanyAsync(company);
        }

        public async Task AddOwnersAsync(int companyId, IEnumerable<OwnerDTO> ownerDtos)
        {
            var owners = _mapper.Map<IEnumerable<Owner>>(ownerDtos);
            await _companyRepository.AddOwnersAsync(companyId, owners);
        }

        public async Task<bool> CheckSocialSecurityNumberAsync(string ssn)
        {
            return await _ssnService.ValidateSsnAsync(ssn);
        }

        public async Task<OwnerDTO> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            var owner = await _companyRepository.GetOwnerByIdAsync(companyId, ownerId);
            return _mapper.Map<OwnerDTO>(owner);
        }
    }
}

