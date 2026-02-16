namespace SyntroVaccPApp.DTOs.Reports
{
    public class OverdueReportDto
    { 
        public string PatientName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public string FacilityName { get; set; } = string.Empty;
        public int LastDoseNumber { get; set; }
        public DateTime? DueDate { get; set; } 
        public string AdministeredBy { get; set; } = string.Empty;
        public int DaysOverdue => DueDate.HasValue
        ? (DateTime.Today - DueDate.Value).Days
        : 0;
    }
}
