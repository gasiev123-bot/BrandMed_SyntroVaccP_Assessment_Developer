using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.ExternalEntityMap
{
    public class ExternalEntityMapCreateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Local entity type cannot exceed 100 characters.")]
        public string LocalEntityType { get; set; } = string.Empty;  

        [Required]
        public int LocalEntityId { get; set; }  

        [Required]
        [StringLength(100, ErrorMessage = "External entity ID cannot exceed 100 characters.")]
        public string ExternalEntityId { get; set; } = string.Empty;  

        [Required]
        [StringLength(100, ErrorMessage = "External system code cannot exceed 100 characters.")]
        public string ExternalSystemCode { get; set; } = string.Empty;  
    }
}
