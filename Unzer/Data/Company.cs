using System;
using System.ComponentModel.DataAnnotations;

namespace Unzer.Data
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Country { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Owner> Owners { get; set; }
    }
}

