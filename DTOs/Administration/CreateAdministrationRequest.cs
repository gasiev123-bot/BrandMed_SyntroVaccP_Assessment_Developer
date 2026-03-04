namespace SyntroVaccPApp.DTOs.Administration
{
    public class CreateAdministrationRequest
    {
        public int PatientId { get; set; }
        public int VaccineId { get; set; }
        public int? BatchId { get; set; }
        public int DoseNumber { get; set; }
        public DateTime AdministeredOn { get; set; }
        public string AdministeredBy { get; set; } = string.Empty;
        public int? FacilityId { get; set; } 
        public string? Notes { get; set; }
    }

}
