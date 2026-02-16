using AutoMapper;
using SyntroVaccPApp.DTOs.Administration;
using SyntroVaccPApp.DTOs.AuditLog;
using SyntroVaccPApp.DTOs.Patient;
using SyntroVaccPApp.DTOs.Vaccine;
using SyntroVaccPApp.DTOs.VaccineBatch;
using Models = SyntroVaccPApp.Models;

namespace SyntroVaccPApp.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.AuditLog, AuditLogReadDto>()
             .ForMember(dest => dest.ChangedBy, opt => opt.MapFrom(src => src.ChangedBy));
             
            CreateMap<Models.Patient, PatientUpdateDto>()
                .ForMember(d => d.IdentifierType,
                o => o.MapFrom(s => s.PatientIdentifiers != null && s.PatientIdentifiers.Any()
                ? s.PatientIdentifiers.First().IdentifierType : null))
                .ForMember(d => d.IdentifierValue,
                o => o.MapFrom(s => s.PatientIdentifiers != null && s.PatientIdentifiers.Any()
                ? s.PatientIdentifiers.First().IdentifierValue : null));

            CreateMap<PatientCreateDto, Models.Patient>();
 
            CreateMap<PatientUpdateDto, Models.Patient>()
                .ForMember(dest => dest.PatientIdentifiers, opt => opt.Ignore());

            CreateMap<Models.Vaccine, VaccineReadDto>();
            CreateMap<VaccineCreateDto, Models.Vaccine>(); 
            CreateMap<Models.Vaccine, VaccineUpdateDto>().ReverseMap();

            CreateMap<Models.VaccineBatch, VaccineBatchReadDto>();
            CreateMap<VaccineBatchCreateDto, Models.VaccineBatch>(); 
            CreateMap<Models.VaccineBatch, VaccineBatchUpdateDto>().ReverseMap();

            CreateMap<Models.Administration, AdministrationReadDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src =>
                    src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : "N/A"))
                .ForMember(dest => dest.VaccineName, opt => opt.MapFrom(src =>
                    src.Vaccine != null ? src.Vaccine.Name : "N/A"))
                .ForMember(dest => dest.ClinicianName, opt => opt.MapFrom(src =>
                    src.Clinician != null ? $"{src.Clinician.FirstName} {src.Clinician.LastName}" : "N/A"))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src =>
                    src.Facility != null ? src.Facility.FacilityName : "N/A"))
                .ForMember(dest => dest.NextDoseDue, opt => opt.MapFrom(src =>
                    (src.Vaccine != null
                        && src.DoseNumber < src.Vaccine.DosesRequired
                        && src.Vaccine.DoseIntervalDays.HasValue)
                    ? src.AdministeredOn.AddDays(src.Vaccine.DoseIntervalDays.Value)
                    : (DateTime?)null));

            CreateMap<AdministrationCreateDto, Models.Administration>();
            CreateMap<AdministrationUpdateDto, Models.Administration>().ReverseMap();
        }
    }
}
