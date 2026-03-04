using System;

namespace SyntroVaccPApp.DTOs.Patient
{
    public class PatientReadDto
    {
        public int PatientId { get; set; }   
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty; 
        public DateTime DateOfBirth { get; set; } 
        public string Sex { get; set; } = string.Empty; 
        public string? ExternalPatientId { get; set; } 
        public string? ExternalSystemCode { get; set; }
    }
}
