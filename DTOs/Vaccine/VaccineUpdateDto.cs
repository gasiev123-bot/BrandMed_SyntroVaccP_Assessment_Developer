using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Vaccine
{
    public class VaccineUpdateDto
    {
        [Required]
        public int VaccineId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Vaccine name cannot exceed 100 characters.")]
        [Display(Name = "Vaccine name")]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [Range(1, 10, ErrorMessage = "Number of doses must be between 1 and 10.")]
        public int DosesRequired { get; set; }

        public bool IsActive { get; set; }

        [RegularExpression(@"^[A-Z0-9]{2,10}$",
            ErrorMessage = "CVX Code must be 2–10 uppercase alphanumeric characters.")]
        public string? CVXCode { get; set; }

        [StringLength(10)]
        public string? ICD11Code { get; set; }

        [StringLength(10)]
        public string? CPTCode { get; set; } 
        public int? DoseIntervalDays { get; set; }
    }

}
