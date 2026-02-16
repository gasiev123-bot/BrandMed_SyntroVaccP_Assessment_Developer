using System;

namespace SyntroVaccPApp.DTOs.Clinician
{
    public class ClinicianReadDto
    {
        public int ClinicianId { get; set; }   

        public string RegistrationNumber { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;  

        public string Specialty { get; set; } = string.Empty;  

        public int FacilityId { get; set; }  

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
