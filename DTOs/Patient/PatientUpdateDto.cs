using System;
using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientUpdateDto
    {
        [Required]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }  

        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression("^(Male|Female|Other)$")]
        public string? Sex { get; set; }

        [Required(ErrorMessage = "Please select a country.")]
        [StringLength(2)]
        public string? CountryCode { get; set; }
        public string? IdentifierType { get; set; } 
        public string? IdentifierValue { get; set; }  

        [StringLength(100, ErrorMessage = "External Patient ID cannot exceed 100 characters.")]
        public string? ExternalPatientId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(100, ErrorMessage = "Modified by cannot exceed 100 characters.")]
        public string? ModifiedBy { get; set; }

        [StringLength(100, ErrorMessage = "External System Code cannot exceed 100 characters.")]
        public string? ExternalSystemCode { get; set; }
    }
}
