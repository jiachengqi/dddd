using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unzer.Data.DTO;
using Unzer.Service;

namespace Unzer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanies()
        {
            var companies = await _companyService.GetCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDTO>> GetCompany(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDTO>> CreateCompany([FromBody] CompanyDTO companyDto)
        {
            var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
            _logger.LogInformation("createCompany request completed successfully for User {UserId}. Company ID: {CompanyId}", User.Identity.Name, createdCompany.Id);

            return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyDTO companyDto)
        {
            await _companyService.UpdateCompanyAsync(id, companyDto);
            _logger.LogInformation("updateCompany request completed successfully for User {UserId}. Company ID: {CompanyId}", User.Identity.Name, id);

            return NoContent();
        }

        [HttpPost("{id}/owners")]
        public async Task<ActionResult> AddOwners(int id, [FromBody] IEnumerable<OwnerDTO> ownerDtos)
        {
            await _companyService.AddOwnersAsync(id, ownerDtos);
            _logger.LogInformation("addOwners request completed successfully for User {UserId}. Company ID: {CompanyId}", User.Identity.Name, id);

            return NoContent();
        }

        [HttpPost("{id}/owner")]
        public async Task<ActionResult> AddOwner(int id, [FromBody] OwnerDTO ownerDto)
        {
            await _companyService.AddOwnerAsync(id, ownerDto);
            _logger.LogInformation("addOwner request completed successfully for User {UserId}. Company ID: {CompanyId}", User.Identity.Name, id);

            return NoContent();
        }

        [HttpGet("CheckSSN/{ssn}")]
        [Authorize]
        public async Task<ActionResult<bool>> CheckSSN(string ssn)
        {
            var isValid = await _companyService.CheckSocialSecurityNumberAsync(ssn);
            return Ok(new { IsValid = isValid });
        }

        [HttpGet("{companyId}/owners/{ownerId}/ssn")]
        [Authorize(Policy = "CanReadSSN")]
        public async Task<ActionResult<string>> GetOwnerSSN(int companyId, int ownerId)
        {
            var owner = await _companyService.GetOwnerByIdAsync(companyId, ownerId);
            return Ok(new { owner.Name, owner.SocialSecurityNumber });
        }
    }
}
