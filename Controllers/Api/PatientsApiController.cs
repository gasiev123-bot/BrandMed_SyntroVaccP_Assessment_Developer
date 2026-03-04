using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Patient;
using SyntroVaccPApp.DTOs.Administration;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers.Api
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public PatientsApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        // ------------------------------
        // GET: api/patients
        // Optional search query: ?search=...
        // ------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientReadDto>>> GetPatients([FromQuery] string? search)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    p.PatientIdentifiers!.Any(pi => pi.IdentifierValue!.Contains(search))
                );
            }

            var result = await query
                .Select(p => new PatientReadDto
                {
                    PatientId = p.PatientId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth
                })
                .ToListAsync();

            return Ok(result);
        }

        // ------------------------------
        // GET: api/patients/{patientId}
        // ------------------------------
        [HttpGet("{patientId:int}")]
        public async Task<ActionResult<PatientReadDto>> GetPatientById(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return NotFound();

            return Ok(new PatientReadDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth
            });
        }

        // ------------------------------
        // POST: api/patients
        // ------------------------------
        [HttpPost]
        public async Task<ActionResult<PatientReadDto>> CreatePatient([FromBody] PatientCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var resultDto = new PatientReadDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth
            };

            return CreatedAtAction(nameof(GetPatientById), new { patientId = patient.PatientId }, resultDto);
        }

        // ------------------------------
        // GET: api/patients/{patientId}/immunisations
        // ------------------------------
        [HttpGet("{patientId:int}/immunisations")]
        public async Task<IActionResult> GetPatientImmunisations(int patientId)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == patientId);
            if (!patientExists) return NotFound("Patient not found.");

            var administrations = await _context.Administrations
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Vaccine)
                .Include(a => a.Facility)
                .Include(a => a.Clinician)
                .OrderBy(a => a.AdministeredOn)
                .ToListAsync();

            var result = administrations.Select(a =>
            {
                DateTime? nextDoseDue = null;
                string? status = null;

                if (a.Vaccine != null && a.DoseNumber < a.Vaccine.DosesRequired)
                {
                    if (a.Vaccine.DoseIntervalDays.HasValue)
                    {
                        nextDoseDue = a.AdministeredOn.AddDays(a.Vaccine.DoseIntervalDays.Value);
                        status = nextDoseDue < DateTime.UtcNow ? "Overdue" : "Pending";
                    }
                    else
                    {
                        status = "Interval not configured";
                    }
                }
                else
                {
                    status = "Course completed";
                }

                return new PatientImmunisationDto
                {
                    AdministrationId = a.AdministrationId,
                    VaccineName = a.Vaccine?.Name ?? "",
                    DoseNumber = a.DoseNumber,
                    AdministeredOn = a.AdministeredOn,
                    Facility = a.Facility?.FacilityName,
                    AdministeredBy = a.Clinician != null ? $"{a.Clinician.FirstName} {a.Clinician.LastName}" : null,
                    NextDoseDueDate = nextDoseDue,
                    NextDoseStatus = status
                };
            }).ToList();

            return Ok(result);
        }
    }
}
