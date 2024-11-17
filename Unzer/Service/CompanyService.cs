using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Unzer.Data;
using Unzer.Data.DTO;
using Unzer.ExceptionHandling;
using Unzer.Repository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

            if (!CanReadSSN())
            {
                foreach (var company in companiesDto)
                {
                    foreach (var owner in company.Owners)
                    {
                        owner.SocialSecurityNumber = null;
                    }
                }
            }

            return companiesDto;
        }

        public async Task<CompanyDTO> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);
            var companyDto = _mapper.Map<CompanyDTO>(company);

            if (!CanReadSSN())
            {
                foreach (var owner in companyDto.Owners)
                {
                    owner.SocialSecurityNumber = null;
                }
            }

            return companyDto;
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
                throw new BadRequestException("Company ID mismatch");
            }

            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.UpdateCompanyAsync(company);
        }

        public async Task AddOwnersAsync(int companyId, IEnumerable<OwnerDTO> ownerDtos)
        {
            var owners = _mapper.Map<IEnumerable<Owner>>(ownerDtos);
            await _companyRepository.AddOwnersAsync(companyId, owners);
        }

        public async Task AddOwnerAsync(int companyId, OwnerDTO ownerDto)
        {
            var owner = _mapper.Map<Owner>(ownerDto);
            await _companyRepository.AddOwnerAsync(companyId, owner);
        }

        public async Task<bool> CheckSocialSecurityNumberAsync(string ssn)
        {
            var isValid = await _ssnService.ValidateSsnAsync(ssn);
            return isValid;
        }

        public async Task<OwnerDTO> GetOwnerByIdAsync(int companyId, int ownerId)
        {
            var owner = await _companyRepository.GetOwnerByIdAsync(companyId, ownerId);
            var ownerDto = _mapper.Map<OwnerDTO>(owner);

            if (!CanReadSSN())
            {
                ownerDto.SocialSecurityNumber = null;
            }

            return ownerDto;
        }

        private bool CanReadSSN()
        {
            var userRoles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role);
            foreach (var role in userRoles)
            {
                if (role.Value == "Admin")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
