using System;
using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Administration
{
    public class AdministrationUpdateDto
    {
        [Required]
        public int AdministrationId { get; set; }

        [Required]
        public int PatientId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        [Required]
        public int VaccineId { get; set; }

        [Required]
        public int? BatchId { get; set; } 

        [Required]
        [Range(1, 20)]
        public int DoseNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AdministeredOn { get; set; }

        [Required]
        public int ClinicianId { get; set; }

        [Required]
        public int? FacilityId { get; set; }

        public string? Notes { get; set; }
    }
}
