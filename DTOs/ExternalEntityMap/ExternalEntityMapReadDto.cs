namespace SyntroVaccPApp.DTOs.ExternalEntityMap
{
    public class ExternalEntityMapReadDto
    {
        public string LocalEntityType { get; set; } = string.Empty; 
        public int LocalEntityId { get; set; } 
        public string ExternalEntityId { get; set; } = string.Empty; 
        public string ExternalSystemCode { get; set; } = string.Empty;
    }
}
