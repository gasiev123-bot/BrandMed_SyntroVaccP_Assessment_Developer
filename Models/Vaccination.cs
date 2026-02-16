using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    
    public class Vaccination
    { 
        public int PatientId { get; set; }
        public int VaccineId { get; set; }

        [Required]
        public int DoseNumber { get; set; }

        [Required]
        public DateTime VaccinationDate { get; set; }

        [Required, MaxLength(200)]
        public string? AdministeredBy { get; set; }

        [Required, MaxLength(200)]
        public string? FacilityName { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [MaxLength(100)]
        public string? ExternalVaccinationId { get; set; }

        [MaxLength(100)]
        public string? ExternalSystemCode { get; set; }

         
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }

        [ForeignKey(nameof(VaccineId))]
        public Vaccine? Vaccine { get; set; }
    }
}
