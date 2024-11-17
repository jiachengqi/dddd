using System;
using System.ComponentModel.DataAnnotations;

namespace Unzer.Data.DTO
{
    public class CompanyDTO
    {      
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Country { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public ICollection<OwnerDTO>? Owners { get; set; }
    }
}

