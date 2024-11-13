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

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanies()
        {
            var companiesDto = await _companyService.GetCompaniesAsync();
            return Ok(companiesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDTO>> GetCompany(int id)
        {
            var companyDto = await _companyService.GetCompanyByIdAsync(id);
            if (companyDto == null)
            {
                return NotFound();
            }
            return Ok(companyDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCompany([FromBody] CompanyDTO companyDto)
        {
            var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
            return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyDTO companyDto)
        {
            try
            {
                await _companyService.UpdateCompanyAsync(id, companyDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Company ID mismatch.");
            }
        }

        [HttpPost("{id}/owners")]
        public async Task<ActionResult> AddOwners(int id, [FromBody] IEnumerable<OwnerDTO> ownerDtos)
        {
            await _companyService.AddOwnersAsync(id, ownerDtos);
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
            if (owner == null)
            {
                return NotFound();
            }

            return Ok(new { owner.Name, owner.SocialSecurityNumber });
        }
    }
}

