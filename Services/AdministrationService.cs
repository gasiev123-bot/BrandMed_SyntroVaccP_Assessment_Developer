using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;
using SyntroVaccPApp.DTOs.Administration;
using Microsoft.EntityFrameworkCore; // for AnyAsync, FirstOrDefaultAsync, etc.

namespace SyntroVaccPApp.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly SyntroVaccPAppDbContext _context;

        public AdministrationService(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateAdministrationRequest request, string changedBy)
        {
            // 1️⃣ Load required entities
            var patient = await _context.Patients.FindAsync(request.PatientId)
                          ?? throw new KeyNotFoundException("Patient not found.");

            var vaccine = await _context.Vaccines.FindAsync(request.VaccineId)
                          ?? throw new KeyNotFoundException("Vaccine not found.");

            VaccineBatch? batch = null;
            if (request.BatchId.HasValue)
            {
                batch = await _context.VaccineBatches.FindAsync(request.BatchId.Value)
                        ?? throw new KeyNotFoundException("Batch not found.");

                if (batch.ExpiryDate < request.AdministeredOn)
                    throw new ArgumentException("Batch has expired.");
            }

            Facility? facility = null;
            if (request.FacilityId.HasValue)
            {
                facility = await _context.Facilities.FindAsync(request.FacilityId.Value)
                           ?? throw new KeyNotFoundException("Facility not found.");
            }

            // 2️⃣ Validate dose number
            if (request.DoseNumber < 1 || request.DoseNumber > vaccine.DosesRequired)
                throw new ArgumentException("Invalid dose number.");

            // 3️⃣ Prevent duplicate dose
            bool duplicateExists = await _context.Administrations.AnyAsync(a =>
                a.PatientId == request.PatientId &&
                a.VaccineId == request.VaccineId &&
                a.DoseNumber == request.DoseNumber
            );

            if (duplicateExists)
                throw new InvalidOperationException("Dose already recorded.");

            // 4️⃣ Create Administration record
            var administration = new Administration
            {
                PatientId = patient.PatientId,
                VaccineId = vaccine.VaccineId,
                BatchId = batch?.BatchId,
                DoseNumber = request.DoseNumber,
                AdministeredOn = request.AdministeredOn,
                FacilityId = facility?.FacilityId,
                Notes = request.Notes,
                CreatedBy = changedBy,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };

            // 5️⃣ Save Administration
            using var transaction = await _context.Database.BeginTransactionAsync();
            _context.Administrations.Add(administration);
            await _context.SaveChangesAsync();

            // 6️⃣ Create Audit Log
            var audit = new AuditLog
            {
                EntityName = "Administration",
                EntityId = administration.AdministrationId,
                Action = "INSERT",
                ChangedBy = changedBy,
                ChangedUtc = DateTime.UtcNow,
                ChangeSummary = System.Text.Json.JsonSerializer.Serialize(request)
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return administration.AdministrationId;
        }
    }
}