using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SyntroVaccPApp.Models
{
    [Table("ExternalEntityMap")]
    public class ExternalEntityMap
    {
        [Key] 
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string LocalEntityType { get; set; } = string.Empty;

        public int LocalEntityId { get; set; }

        [Required, MaxLength(50)]
        public string ExternalEntityId { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string ExternalSystemCode { get; set; } = string.Empty;
    }
 
}
