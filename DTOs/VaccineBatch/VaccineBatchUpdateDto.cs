using System;
using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.VaccineBatch
{
    public class VaccineBatchUpdateDto
    {
        [Required]
        public int BatchId { get; set; } 
        public int VaccineId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Batch number cannot exceed 200 characters.")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public string Manufacturer { get; set; } = "";
    }
}
