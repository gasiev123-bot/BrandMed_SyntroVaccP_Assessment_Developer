namespace SyntroVaccPApp.Models
{
    public class PatientDetailsViewModel
    {
        public Patient Patient { get; set; } = new Patient();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public IEnumerable<Administration> Administrations { get; set; } = Enumerable.Empty<Administration>();
    }

}
