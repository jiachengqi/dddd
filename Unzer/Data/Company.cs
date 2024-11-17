using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unzer.Data
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Country { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        
        public ICollection<Owner> Owners { get; set; }
    }
}

