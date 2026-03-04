using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("Country")]
    public class Country
    {
        [Key, StringLength(2)]
        public string CountryCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string CountryName { get; set; } = string.Empty;
    }
}
