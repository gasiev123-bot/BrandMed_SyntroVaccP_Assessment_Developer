using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientCreateDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^(Male|Female|Other)$")]
        [Display(Name = "Sex")]
        public string Sex { get; set; } = string.Empty; 

        [Required]
        public string? IdentifierType { get; set; }   // "NationalId" | "Passport"

        [Required]
        public string? IdentifierValue { get; set; }  

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100, ErrorMessage = "External Patient ID cannot exceed 100 characters.")]
        public string? ExternalPatientId { get; set; }

        [StringLength(100, ErrorMessage = "External System Code cannot exceed 100 characters.")]
        public string? ExternalSystemCode { get; set; } 
        public string? CountryCode { get; set; }
    }
}
