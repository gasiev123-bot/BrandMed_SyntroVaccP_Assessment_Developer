namespace SyntroVaccPApp.DTOs.Reports
{
    public class OverdueVaccinationDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public int DoseNumber { get; set; }
        public DateTime LastDoseDate { get; set; }
        public DateTime NextDoseDueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
