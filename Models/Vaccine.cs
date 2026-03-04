using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models 
{
    [Table("Vaccine")]
    public class Vaccine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VaccineId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required")]
        [StringLength(100)]
        [Display(Name = "Vaccine Name")]
        [Column("VaccineName")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Manufacturer")]
        public string? Manufacturer { get; set; }

        [Required(ErrorMessage = "Number of doses is required")]
        [Range(1, 10, ErrorMessage = "Number of doses must be between 1 and 10")]
        [Display(Name = "Number of Doses")]
        [Column("NumberOfDoses")]
        public int DosesRequired { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "varchar(10)")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Invalid CVX code format")]
        [Display(Name = "CVX Code")]
        public string? CVXCode { get; set; }

        [Column(TypeName = "varchar(10)")]
        [RegularExpression(@"^[A-Z0-9\.]+$", ErrorMessage = "Invalid ICD-11 code format")]
        [Display(Name = "ICD-11 Code")]
        public string? ICD11Code { get; set; }

        [Column(TypeName = "varchar(10)")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Invalid CPT code format")]
        [Display(Name = "CPT Code")]
        public string? CPTCode { get; set; }

        [Display(Name = "Dose Interval (Days)")]
        [Range(1, 365, ErrorMessage = "Dose interval must be between 1 and 365 days")]
        public int? DoseIntervalDays { get; set; }

    }
}
