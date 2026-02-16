using System;
using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Administration
{
    public class AdministrationCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        public string PatientName { get; set; }  = string.Empty;

        [Required(ErrorMessage = "Please select a vaccine.")]
        public int? VaccineId { get; set; }

        [Required(ErrorMessage = "Please select a batch number.")] 
        public int? BatchId { get; set; }  

        [Required]
        [Range(1, 10, ErrorMessage = "Dose number must be between 1 and 10.")]
        public int DoseNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AdministeredOn { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Clinician is required.")]
        public int ClinicianId { get; set; }
         
        public int? FacilityId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

