using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Clinician
{
    public class ClinicianCreateDto
    {
        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(200, ErrorMessage = "Registration number cannot exceed 200 characters.")]
        public string RegistrationNumber { get; set; } = null!;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(200, ErrorMessage = "First name cannot exceed 200 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(200, ErrorMessage = "Last name cannot exceed 200 characters.")]
        public string LastName { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }  

        [StringLength(200, ErrorMessage = "Specialty cannot exceed 200 characters.")]
        public string? Specialty { get; set; }   

        [Required(ErrorMessage = "Facility ID is required.")]
        public int FacilityId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(510, ErrorMessage = "Email cannot exceed 510 characters.")]
        public string? Email { get; set; }  

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100, ErrorMessage = "Phone number cannot exceed 100 characters.")]
        public string? Phone { get; set; }  

        public bool IsActive { get; set; } = true;  
    }
}
