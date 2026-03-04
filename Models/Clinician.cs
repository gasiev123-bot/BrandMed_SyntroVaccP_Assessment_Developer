using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("Clinician")]
    public class Clinician
    {
        [Key]
        public int ClinicianId { get; set; }

        [Required, MaxLength(200)]
        public string? RegistrationNumber { get; set; }

        [Required, MaxLength(200)]
        public string? FirstName { get; set; }

        [Required, MaxLength(200)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(200)]
        public string? Specialty { get; set; }

        public int? FacilityId { get; set; }

        [MaxLength(510)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; }
    }
}
