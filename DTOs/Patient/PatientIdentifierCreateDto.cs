using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientIdentifierCreateDto
    {
        [Required]
        public int PatientId { get; set; }  

        [Required]
        [StringLength(50, ErrorMessage = "Identifier type cannot exceed 50 characters.")]
        public string IdentifierType { get; set; } = string.Empty;  

        [Required]
        [StringLength(100, ErrorMessage = "Identifier value cannot exceed 100 characters.")]
        public string IdentifierValue { get; set; } = string.Empty; 

        [StringLength(100)]
        public string? ExternalSystemCode { get; set; }  
    }
}
