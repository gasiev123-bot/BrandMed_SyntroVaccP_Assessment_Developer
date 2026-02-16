using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Vaccine
{
    public class VaccineCreateDto
    {
        public int VaccineId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Vaccine name")]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "Manufacturer")]
        public string? Manufacturer { get; set; }

        [Range(1, 10)]
        [Display(Name = "Doses required")]
        public int DosesRequired { get; set; } 
        public bool IsActive { get; set; }

        [RegularExpression(@"^[A-Z0-9]{2,10}$")]
        public string? CVXCode { get; set; } 
        public string? ICD11Code { get; set; }
        public string? CPTCode { get; set; }

        [Display(Name = "Dose Interval (Days)")]
        public int? DoseIntervalDays { get; set; }
    }
}
