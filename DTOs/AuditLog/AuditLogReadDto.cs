using System;

namespace SyntroVaccPApp.DTOs.AuditLog
{
    public class AuditLogReadDto
    {
        public int AuditLogId { get; set; }

        public string EntityName { get; set; } = string.Empty; 

        public int EntityId { get; set; }  

        public string Action { get; set; } = string.Empty; 

        public string? OldValue { get; set; }  

        public string? NewValue { get; set; }  

        public DateTime ActionDate { get; set; }
         
        public string ChangedBy { get; set; } = string.Empty;

        public DateTime? ChangedUtc { get; set; }  
        public string? ChangeSummary { get; set; }
    }
}
