using SyntroVaccPApp.DTOs.Administration;
public interface IAdministrationService
{
        Task<int> CreateAsync(CreateAdministrationRequest request, string changedBy);
}
 