using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("Administration")]
    public class Administration
    {
        [Key]
        public int AdministrationId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int? VaccineId { get; set; }

        public int? FacilityId { get; set; } // Optional per spec

        [Required]
        public int ClinicianId { get; set; }

        [Required]
        public int DoseNumber { get; set; }

        [Required]
        public DateTime AdministeredOn { get; set; }

        public int? BatchId { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedUtc { get; set; }

        public DateTime? UpdatedUtc { get; set; }

        [Required, MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
 
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }

        [ForeignKey(nameof(VaccineId))]
        public Vaccine? Vaccine { get; set; }

        [ForeignKey(nameof(FacilityId))]
        public Facility? Facility { get; set; }

        [ForeignKey(nameof(ClinicianId))]
        public Clinician? Clinician { get; set; }

        [Required]
        [MaxLength(100)]  
        public string BatchNumber { get; set; } = string.Empty;

        [ForeignKey(nameof(BatchId))]
        public VaccineBatch? Batch { get; set; }
 
        [NotMapped]
        public DateTime? NextDoseDue => Vaccine != null && DoseNumber < Vaccine.DosesRequired && Vaccine.DoseIntervalDays.HasValue
            ? AdministeredOn.AddDays(Vaccine.DoseIntervalDays.Value)
            : null;
    }
}
