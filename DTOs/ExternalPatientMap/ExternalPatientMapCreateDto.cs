using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.ExternalPatientMap
{
    public class ExternalPatientMapCreateDto
    {
        [Required]
        public int PatientId { get; set; } 

        [Required]
        [StringLength(100, ErrorMessage = "External patient ID cannot exceed 100 characters.")]
        public string ExternalPatientId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "External system code cannot exceed 100 characters.")]
        public string ExternalSystemCode { get; set; } = string.Empty;  
    }
}
