using System.ComponentModel.DataAnnotations;

namespace SyntroVaccPApp.DTOs.Country
{
    public class CountryCreateDto
    {
        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be 2 characters.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "Country name cannot exceed 200 characters.")]
        public string CountryName { get; set; } = string.Empty;
    }
}
