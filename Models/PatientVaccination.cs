using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    /// <summary>
    /// In-memory model representing a patient's vaccination.
    /// Not persisted as a separate table; maps to Administration table.
    /// </summary>
    [NotMapped] // EF Core will ignore this model when creating the database
    public class PatientVaccination
    {
        [Key]
        public int PatientVaccinationId { get; set; }  

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int VaccineId { get; set; }

        [Required]
        public int DoseNumber { get; set; }

        [Required]
        public DateTime VaccinationDate { get; set; }

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
