using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("PatientIdentifier")]
    public class PatientIdentifier
    {
        [Key]
        public int PatientIdentifierId { get; set; } 
        public int PatientId { get; set; }

        [Required, MaxLength(40)]
        public string? IdentifierType { get; set; }

        [Required, MaxLength(60)]
        public string? IdentifierValue { get; set; }

        [Required, StringLength(2)]
        public string? CountryCode { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreatedDate { get; set; }

        [MaxLength(100)]
        public string? ExternalIdentifierId { get; set; } 

        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }

        
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
    }
}
