using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Administration;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers.Api
{
    [ApiController]
    [Route("api/patients/{patientId}/administrations")]
    [Authorize(Roles = "Admin,Clerk,Clinician")]
    public class AdministrationsApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public AdministrationsApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        // ------------------------------
        // GET: api/patients/{patientId}/administrations
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAdministrations(int patientId)
        {
            var administrations = await _context.Administrations
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Vaccine)
                .Include(a => a.Clinician)
                .Include(a => a.Facility)
                .OrderByDescending(a => a.AdministeredOn)
                .Select(a => new AdministrationReadDto
                {
                    AdministrationId = a.AdministrationId,
                    PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : "",
                    VaccineName = a.Vaccine!.Name,
                    DoseNumber = a.DoseNumber,
                    AdministeredOn = a.AdministeredOn,
                    ClinicianName = a.Clinician != null ? $"{a.Clinician.FirstName} {a.Clinician.LastName}" : "",
                    FacilityName = a.Facility!.FacilityName,
                    BatchNumber = a.BatchNumber
                })
                .ToListAsync();

            return Ok(administrations);
        }

        // ------------------------------
        // GET: api/patients/{patientId}/administrations/{id}
        // ------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAdministration(int patientId, int id)
        {
            var admin = await _context.Administrations
                .Include(a => a.Vaccine)
                .Include(a => a.Clinician)
                .Include(a => a.Facility)
                .FirstOrDefaultAsync(a => a.AdministrationId == id && a.PatientId == patientId);

            if (admin == null) return NotFound();

            var dto = new AdministrationReadDto
            {
                AdministrationId = admin.AdministrationId,
                PatientName = admin.Patient != null ? $"{admin.Patient.FirstName} {admin.Patient.LastName}" : "",
                VaccineName = admin.Vaccine?.Name,
                DoseNumber = admin.DoseNumber,
                AdministeredOn = admin.AdministeredOn,
                ClinicianName = admin.Clinician != null ? $"{admin.Clinician.FirstName} {admin.Clinician.LastName}" : "",
                FacilityName = admin.Facility?.FacilityName,
                BatchNumber = admin.BatchNumber
            };

            return Ok(dto);
        }

        // ------------------------------
        // POST: api/patients/{patientId}/administrations
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateAdministration(int patientId, [FromBody] AdministrationCreateDto dto)
        {
            if (dto.PatientId != patientId)
                return BadRequest("PatientId mismatch.");

            // Validate patient exists
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return NotFound("Patient not found.");

            // Validate vaccine exists
            var vaccine = await _context.Vaccines.FindAsync(dto.VaccineId);
            if (vaccine == null) return BadRequest("Vaccine not found.");

            // Auto-calculate dose
            if (dto.DoseNumber < 1)
            {
                var lastDose = await _context.Administrations
                    .Where(a => a.PatientId == patientId && a.VaccineId == dto.VaccineId)
                    .MaxAsync(a => (int?)a.DoseNumber) ?? 0;

                dto.DoseNumber = lastDose + 1;
            }

            // Duplicate dose prevention
            bool duplicate = await _context.Administrations.AnyAsync(a =>
                a.PatientId == patientId &&
                a.VaccineId == dto.VaccineId &&
                a.DoseNumber == dto.DoseNumber);

            if (duplicate)
                return Conflict("This dose has already been administered to this patient.");

            // Batch validation
            string batchNumber = "";
            if (dto.BatchId.HasValue)
            {
                var batch = await _context.VaccineBatches.FindAsync(dto.BatchId.Value);
                if (batch == null) return BadRequest("Selected batch does not exist.");
                if (batch.VaccineId != dto.VaccineId) return BadRequest("Batch does not match vaccine.");
                if (dto.AdministeredOn.Date > batch.ExpiryDate.Date) return BadRequest("Cannot administer from expired batch.");

                batchNumber = batch.BatchNumber;
            }

            var admin = new Administration
            {
                PatientId = patientId,
                VaccineId = dto.VaccineId,
                BatchId = dto.BatchId,
                BatchNumber = batchNumber,
                DoseNumber = dto.DoseNumber,
                AdministeredOn = dto.AdministeredOn,
                ClinicianId = dto.ClinicianId,
                FacilityId = dto.FacilityId,
                Notes = dto.Notes,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "API",
                UpdatedBy = User.Identity?.Name ?? "API"
            };

            _context.Administrations.Add(admin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdministration), new { patientId = patientId, id = admin.AdministrationId }, admin);
        }

        // ------------------------------
        // PUT: api/patients/{patientId}/administrations/{id}
        // ------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAdministration(int patientId, int id, [FromBody] AdministrationUpdateDto dto)
        {
            var admin = await _context.Administrations
                .FirstOrDefaultAsync(a => a.AdministrationId == id && a.PatientId == patientId);

            if (admin == null) return NotFound();

            // Validate vaccine and clinician
            if (!await _context.Vaccines.AnyAsync(v => v.VaccineId == dto.VaccineId))
                return BadRequest("Vaccine not found.");
            if (!await _context.Clinicians.AnyAsync(c => c.ClinicianId == dto.ClinicianId))
                return BadRequest("Clinician not found.");

            // Batch validation
            string batchNumber = "";
            if (dto.BatchId.HasValue)
            {
                var batch = await _context.VaccineBatches.FindAsync(dto.BatchId.Value);
                if (batch == null) return BadRequest("Batch not found.");
                if (batch.VaccineId != dto.VaccineId) return BadRequest("Batch does not match vaccine.");
                if (dto.AdministeredOn.Date > batch.ExpiryDate.Date) return BadRequest("Cannot administer from expired batch.");

                batchNumber = batch.BatchNumber;
            }

            // Update fields
            admin.VaccineId = dto.VaccineId;
            admin.BatchId = dto.BatchId;
            admin.BatchNumber = batchNumber;
            admin.DoseNumber = dto.DoseNumber;
            admin.AdministeredOn = dto.AdministeredOn;
            admin.ClinicianId = dto.ClinicianId;
            admin.FacilityId = dto.FacilityId;
            admin.Notes = dto.Notes;
            admin.UpdatedUtc = DateTime.UtcNow;
            admin.UpdatedBy = User.Identity?.Name ?? "API";

            await _context.SaveChangesAsync();

            return Ok(admin);
        }

        // ------------------------------
        // DELETE: api/patients/{patientId}/administrations/{id}
        // ------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAdministration(int patientId, int id)
        {
            var admin = await _context.Administrations
                .FirstOrDefaultAsync(a => a.AdministrationId == id && a.PatientId == patientId);

            if (admin == null) return NotFound();

            _context.Administrations.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
