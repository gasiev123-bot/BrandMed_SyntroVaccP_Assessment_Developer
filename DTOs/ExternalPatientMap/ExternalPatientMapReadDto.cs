namespace SyntroVaccPApp.DTOs.ExternalPatientMap
{
    public class ExternalPatientMapReadDto
    {
        public int PatientId { get; set; } 

        public string ExternalPatientId { get; set; } = string.Empty;

        public string ExternalSystemCode { get; set; } = string.Empty;
    }
}
