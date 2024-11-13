using System;
using System.ComponentModel.DataAnnotations;

namespace Unzer.Data
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SocialSecurityNumber { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}

