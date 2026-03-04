namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientImmunisationDto
    {
        public int AdministrationId { get; set; }
        public string VaccineName { get; set; } = string.Empty;
        public int DoseNumber { get; set; }
        public DateTime AdministeredOn { get; set; }
        public string? Facility { get; set; }
        public string? AdministeredBy { get; set; }

        public DateTime? NextDoseDueDate { get; set; }
        public string? NextDoseStatus { get; set; }
    }

}
