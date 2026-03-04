namespace SyntroVaccPApp.DTOs.Reports
{
    public class VaccinationReportDto
    {
        // Grouping & Filtering Labels
        public string VaccineName { get; set; } = string.Empty;
        public string FacilityName { get; set; } = string.Empty; 
        public int TotalDoses { get; set; } 
        public string? Manufacturer { get; set; }  
        public string? BatchNumber { get; set; }  
 
        public DateTime? DateAdministered { get; set; } 
    }
}
