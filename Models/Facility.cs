using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models 
{
    [Table("Facility")]
    public class Facility
    {
        [Key]
        public int FacilityId { get; set; }

        [Required, MaxLength(100)]
        public string? FacilityCode { get; set; }

        [Required, MaxLength(400)]
        public string FacilityName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Province { get; set; }

        [StringLength(3)]
        public string? CountryCode { get; set; } 
        public bool IsActive { get; set; } 
        public ICollection<Administration>? Administrations { get; set; }

    }
}
