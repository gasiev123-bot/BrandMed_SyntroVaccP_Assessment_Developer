namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientIdentifierReadDto
    {
        public int PatientIdentifierId { get; set; }   
        public int PatientId { get; set; }  
        public string IdentifierType { get; set; } = string.Empty;  
        public string IdentifierValue { get; set; } = string.Empty;   
        public string? ExternalSystemCode { get; set; }  
    }
}
