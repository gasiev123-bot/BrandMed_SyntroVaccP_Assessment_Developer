using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models 
{
    [Table("Patient", Schema = "dbo")]
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; } 
        
        [MaxLength(6)]
        public string Sex { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required, MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Column("ModifiedDate")]  
        public DateTime? ModifiedDate { get; set; }

        [Column("ModifiedBy")]  
        public string? ModifiedBy { get; set; } 

        [MaxLength(100)]
        public string? ExternalPatientId { get; set; }

        [MaxLength(100)]
        public string? ExternalSystemCode { get; set; }  
        public ICollection<PatientIdentifier>? PatientIdentifiers { get; set; } 
        public ICollection<Administration>? Administrations { get; set; }

    }
}
