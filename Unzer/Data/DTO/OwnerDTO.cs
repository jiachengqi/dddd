using System;
using System.ComponentModel.DataAnnotations;

namespace Unzer.Data.DTO
{
    public class OwnerDTO
    {
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SocialSecurityNumber { get; set; }
    }
}

