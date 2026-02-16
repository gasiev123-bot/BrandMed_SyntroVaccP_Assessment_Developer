using System;

namespace SyntroVaccPApp.DTOs.Administration
{
    public class AdministrationReadDto
    {
        public int AdministrationId { get; set; } 
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? VaccineName { get; set; }
        public string? ClinicianName { get; set; }
        public string? FacilityName { get; set; }
        public DateTime AdministeredOn { get; set; }
        public int VaccineId { get; set; } 
        public int? BatchId { get; set; }
        public string? BatchNumber { get; set; } 
        public int DoseNumber { get; set; } 
        public string AdministeredBy { get; set; } = string.Empty;
        public int FacilityId { get; set; } 
        public string? Notes { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? NextDoseDue { get; set; }
        public int DosesRequired { get; set; }
        public int? DoseIntervalDays { get; set; }
        public string NextDoseSuggestion { get; set; } = string.Empty;


    }
}
