using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("ExternalPatientMap")]
    public class ExternalPatientMap
    {
        [Key]
        public int Id { get; set; } 

        public int LocalPatientId { get; set; }

        [Required, MaxLength(100)]
        public string? ExternalPatientId { get; set; }

        [Required, MaxLength(100)]
        public string? ExternalSystemCode { get; set; }
    }
}
