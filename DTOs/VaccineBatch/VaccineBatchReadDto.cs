using System;

namespace SyntroVaccPApp.DTOs.VaccineBatch
{
    public class VaccineBatchReadDto
    {
        public int BatchId { get; set; } 
        public int VaccineId { get; set; } 
        public string BatchNumber { get; set; } = string.Empty; 
        public DateTime ExpiryDate { get; set; }  
        public string VaccineName { get; set; } = string.Empty; 
    }
}
