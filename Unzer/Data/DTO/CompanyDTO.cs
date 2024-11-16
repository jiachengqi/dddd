using System;
namespace Unzer.Data.DTO
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public ICollection<OwnerDTO>? Owners { get; set; }
    }
}

