using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unzer.Data.DTO;
using Unzer.ExceptionHandling;
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

        /// <summary>
        /// Retrieves a list of all companies.
        /// </summary>
        /// <returns>List of CompanyDTO objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanies()
        {
            _logger.LogInformation("GET api/Companies called.");

            try
            {
                var companiesDto = await _companyService.GetCompaniesAsync();
                _logger.LogInformation("Successfully retrieved {Count} companies.", companiesDto?.ToString()?.Length ?? 0);
                return Ok(companiesDto);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while fetching companies.");
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching companies.");
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching companies." });
            }
        }

        /// <summary>
        /// Retrieves a specific company by ID.
        /// </summary>
        /// <param name="id">Company ID.</param>
        /// <returns>CompanyDTO object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDTO>> GetCompany(int id)
        {
            _logger.LogInformation("GET api/Companies/{Id} called.", id);

            try
            {
                var companyDto = await _companyService.GetCompanyByIdAsync(id);
                _logger.LogInformation("Successfully retrieved company with ID {Id}.", id);
                return Ok(companyDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {Id} not found.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while fetching company with ID {Id}.", id);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching company with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving the company." });
            }
        }

        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="companyDto">CompanyDTO object.</param>
        /// <returns>Created CompanyDTO object.</returns>
        [HttpPost]
        public async Task<ActionResult<CompanyDTO>> CreateCompany([FromBody] CompanyDTO companyDto)
        {
            _logger.LogInformation("POST api/Companies called with CompanyName: {CompanyName}.", companyDto.Name);

            try
            {
                var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
                _logger.LogInformation("Successfully created company with ID {Id}.", createdCompany.Id);
                return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while creating company: {CompanyName}.", companyDto.Name);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating company: {CompanyName}.", companyDto.Name);
                return StatusCode(500, new { Message = "An unexpected error occurred while creating the company." });
            }
        }

        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="id">Company ID.</param>
        /// <param name="companyDto">CompanyDTO object.</param>
        /// <returns>No Content.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyDTO companyDto)
        {
            _logger.LogInformation("PUT api/Companies/{Id} called.", id);

            try
            {
                await _companyService.UpdateCompanyAsync(id, companyDto);
                _logger.LogInformation("Successfully updated company with ID {Id}.", id);
                return NoContent();
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request while updating company with ID {Id}.", id);
                return BadRequest(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while updating company with ID {Id}.", id);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating company with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while updating the company." });
            }
        }

        /// <summary>
        /// Adds multiple owners to a company.
        /// </summary>
        /// <param name="id">Company ID.</param>
        /// <param name="ownerDtos">List of OwnerDTO objects.</param>
        /// <returns>No Content.</returns>
        [HttpPost("{id}/owners")]
        public async Task<ActionResult> AddOwners(int id, [FromBody] IEnumerable<OwnerDTO> ownerDtos)
        {
            _logger.LogInformation("POST api/Companies/{Id}/owners called with {Count} owners.", id, ownerDtos?.ToString()?.Length ?? 0);

            try
            {
                await _companyService.AddOwnersAsync(id, ownerDtos);
                _logger.LogInformation("Successfully added owners to company with ID {Id}.", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {Id} not found while adding owners.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while adding owners to company with ID {Id}.", id);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding owners to company with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while adding owners." });
            }
        }

        /// <summary>
        /// Adds a single owner to a company.
        /// </summary>
        /// <param name="id">Company ID.</param>
        /// <param name="ownerDto">OwnerDTO object.</param>
        /// <returns>No Content.</returns>
        [HttpPost("{id}/owner")]
        public async Task<ActionResult> AddOwner(int id, [FromBody] OwnerDTO ownerDto)
        {
            _logger.LogInformation("POST api/Companies/{Id}/owner called with OwnerName: {OwnerName}.", id, ownerDto.Name);

            try
            {
                await _companyService.AddOwnerAsync(id, ownerDto);
                _logger.LogInformation("Successfully added owner to company with ID {Id}.", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Company with ID {Id} not found while adding an owner.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while adding an owner to company with ID {Id}.", id);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding an owner to company with ID {Id}.", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while adding the owner." });
            }
        }

        /// <summary>
        /// Checks the validity of a Social Security Number (SSN).
        /// </summary>
        /// <param name="ssn">Social Security Number.</param>
        /// <returns>Boolean indicating if the SSN is valid.</returns>
        [HttpGet("CheckSSN/{ssn}")]
        [Authorize]
        public async Task<ActionResult<bool>> CheckSSN(string ssn)
        {
            _logger.LogInformation("GET api/Companies/CheckSSN/{Ssn} called.", ssn);

            try
            {
                var isValid = await _companyService.CheckSocialSecurityNumberAsync(ssn);
                _logger.LogInformation("SSN {Ssn} validation result: {IsValid}.", ssn, isValid);
                return Ok(new { IsValid = isValid });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while validating SSN: {Ssn}.", ssn);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while validating SSN: {Ssn}.", ssn);
                return StatusCode(500, new { Message = "An unexpected error occurred while validating the Social Security Number." });
            }
        }

        /// <summary>
        /// Retrieves an owner's Social Security Number (SSN).
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="ownerId">Owner ID.</param>
        /// <returns>Owner's name and SSN.</returns>
        [HttpGet("{companyId}/owners/{ownerId}/ssn")]
        [Authorize(Policy = "CanReadSSN")]
        public async Task<ActionResult<string>> GetOwnerSSN(int companyId, int ownerId)
        {
            _logger.LogInformation("GET api/Companies/{CompanyId}/owners/{OwnerId}/ssn called.", companyId, ownerId);

            try
            {
                var ownerDto = await _companyService.GetOwnerByIdAsync(companyId, ownerId);
                _logger.LogInformation("Successfully retrieved SSN for owner with ID {OwnerId} in company {CompanyId}.", ownerId, companyId);
                return Ok(new { ownerDto.Name, ownerDto.SocialSecurityNumber });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Owner with ID {OwnerId} not found for company ID {CompanyId}.", ownerId, companyId);
                return NotFound(new { Message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error occurred while fetching SSN for owner with ID {OwnerId} in company {CompanyId}.", ownerId, companyId);
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching SSN for owner with ID {OwnerId} in company {CompanyId}.", ownerId, companyId);
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving the owner's Social Security Number." });
            }
        }
    }
}
