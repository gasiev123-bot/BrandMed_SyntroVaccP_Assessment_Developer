using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyntroVaccPApp.Models
{
    [Table("VaccineBatch")]
    public class VaccineBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchId { get; set; }

        [Required]
        public int VaccineId { get; set; }

        [ForeignKey("VaccineId")]
        public Vaccine? Vaccine { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        
    }
}
