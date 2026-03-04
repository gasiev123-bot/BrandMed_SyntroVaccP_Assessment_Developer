using System;

namespace SyntroVaccPApp.DTOs.Vaccine
{
    public class VaccineReadDto
    {
        public int VaccineId { get; set; }   
        public string Name { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public int DosesRequired { get; set; }
        public bool IsActive { get; set; }
        public string? CVXCode { get; set; }
        public string? ICD11Code { get; set; }
        public string? CPTCode { get; set; }
        public int? DoseIntervalDays { get; set; }  
    }
}
