using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Required, MaxLength(100)]
        public string? EntityName { get; set; }

        public int EntityId { get; set; }

        [Required, MaxLength(40)]
        public string? Action { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }
         
        public DateTime? ChangedUtc { get; set; }

        [Required, MaxLength(100)]
        public string? ChangedBy { get; set; } 
        public string? ChangeSummary { get; set; }  
    }
}
