using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Clinician
{
    public class ClinicianUpdateDto
    {
        [StringLength(200, ErrorMessage = "Registration number cannot exceed 200 characters.")]
        public string? RegistrationNumber { get; set; }

        [StringLength(200, ErrorMessage = "First name cannot exceed 200 characters.")]
        public string? FirstName { get; set; }

        [StringLength(200, ErrorMessage = "Last name cannot exceed 200 characters.")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(200, ErrorMessage = "Specialty cannot exceed 200 characters.")]
        public string? Specialty { get; set; }

        public int? FacilityId { get; set; }  

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(510, ErrorMessage = "Email cannot exceed 510 characters.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100, ErrorMessage = "Phone number cannot exceed 100 characters.")]
        public string? Phone { get; set; }

        public bool? IsActive { get; set; } 
    }
}
