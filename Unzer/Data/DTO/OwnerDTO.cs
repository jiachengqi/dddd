using System;
namespace Unzer.Data.DTO
{
    public class OwnerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SocialSecurityNumber { get; set; }
        // Exclude SSN for certain access groups
    }
}

