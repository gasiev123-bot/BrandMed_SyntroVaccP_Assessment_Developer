using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs;
using SyntroVaccPApp.DTOs.Administration;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers
{
    [Authorize(Roles = "Admin,Clinician,Clerk,Auditor")]
    public class AdministrationController : Controller
    {
        private readonly SyntroVaccPAppDbContext _context;
        private readonly IMapper _mapper;

        public AdministrationController(SyntroVaccPAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =========================================================
        // INDEX (Pagination)
        // =========================================================
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var query = _context.Administrations
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Vaccine)
                .Include(a => a.Clinician)
                .Include(a => a.Facility)
                .Include(a => a.Batch)
                .OrderByDescending(a => a.AdministeredOn);

            var totalCount = await query.CountAsync();

            var administrations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = _mapper.Map<List<AdministrationReadDto>>(administrations);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(dtoList);
        }


        // =========================================================
        // CREATE (GET - Partial)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? patientId)
        {
            await PopulateDropdownsAsync();

            var dto = new AdministrationCreateDto
            {
                AdministeredOn = DateTime.Today
            };

            if (patientId.HasValue)
            {
                // Defensive check: ensure patient exists
                var patient = await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PatientId == patientId.Value);

                if (patient == null)
                {
                    // Redirect back to administration index with a message
                    TempData["AdminError"] = $"Patient with ID {patientId.Value} not found.";
                    return RedirectToAction("Index");
                }

                dto.PatientId = patient.PatientId;
                dto.PatientName = $"{patient.FirstName} {patient.LastName}";
            }

            return PartialView("_CreateAdministrationPartial", dto);
        }


        // =========================================================
        // CREATE (POST - AJAX)
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdministrationCreateDto dto)
        {

            // ===============================
            // Validate Vaccine exists
            // ===============================
            var vaccine = await _context.Vaccines
                .FirstOrDefaultAsync(v => v.VaccineId == dto.VaccineId);

            if (vaccine == null)
            {
                TempData["AdminError"]= $"Selected vaccine {dto.VaccineId}  does not exist.";
               // ModelState.AddModelError("VaccineId", "Selected vaccine does not exist.");
            }
            else 
            {
                // ===============================
                // Auto-calculate next dose
                // ===============================
                var lastDose = await _context.Administrations
                    .Where(a => a.PatientId == dto.PatientId &&
                                a.VaccineId == dto.VaccineId)
                    .MaxAsync(a => (int?)a.DoseNumber) ?? 0;

                dto.DoseNumber = lastDose + 1;

                // Ensure dose does not exceed required doses
                if (dto.DoseNumber > vaccine.DosesRequired)
                {
                    TempData["AdminError"] = $"All required doses for this vaccine {dto.VaccineId}, have already been administered.";
                    /*ModelState.AddModelError("VaccineId",
                        "All required doses for this vaccine have already been administered.");*/
                }
            }

            // ===============================
            // Validate Dose range
            // ===============================
            if (dto.DoseNumber < 1 || dto.DoseNumber > vaccine?.DosesRequired)
            {
                ModelState.AddModelError("DoseNumber",
                    $"Dose must be between 1 and {vaccine?.DosesRequired}.");
            }

            // ===============================
            // Prevent duplicate dose
            // ===============================
            bool duplicate = await _context.Administrations.AnyAsync(a =>
                a.PatientId == dto.PatientId &&
                a.VaccineId == dto.VaccineId &&
                a.DoseNumber == dto.DoseNumber);

            if (duplicate)
            {
                ModelState.AddModelError("DoseNumber",
                    "This dose has already been administered to this patient.");
            }

            // ===============================
            // Ensure previous dose exists
            // ===============================
            if (dto.DoseNumber > 1)
            {
                bool previousExists = await _context.Administrations.AnyAsync(a =>
                    a.PatientId == dto.PatientId &&
                    a.VaccineId == dto.VaccineId &&
                    a.DoseNumber == dto.DoseNumber - 1);

                if (!previousExists)
                {
                    ModelState.AddModelError("DoseNumber",
                        $"Dose {dto.DoseNumber - 1} must be administered first.");
                }
            }

            // ===============================
            // Validate Batch
            // ===============================
            if (dto.BatchId.HasValue)
            {
                var batch = await _context.VaccineBatches
                    .FirstOrDefaultAsync(b => b.BatchId == dto.BatchId.Value);

                if (batch == null)
                {
                    ModelState.AddModelError("BatchId", "Selected batch does not exist.");
                }
                else
                {
                    // Ensure batch belongs to selected vaccine
                    if (batch.VaccineId != dto.VaccineId)
                    {
                        ModelState.AddModelError("BatchId",
                            "Selected batch does not belong to the chosen vaccine.");
                    }

                    // Ensure batch not expired
                    if (dto.AdministeredOn.Date > batch.ExpiryDate.Date)
                    {
                        ModelState.AddModelError("BatchId",
                            "Cannot administer vaccine from an expired batch.");
                    }
                }
            } 

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return PartialView("_CreateAdministrationPartial", dto);
            }

            // Map DTO to entity
            var admin = _mapper.Map<Administration>(dto);

            // ✅ Lookup batch and set BatchNumber
            if (dto.BatchId.HasValue)
            {
                var batch = await _context.VaccineBatches
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BatchId == dto.BatchId.Value);

                admin.BatchNumber = batch != null ? batch.BatchNumber : "";
            }
            else
            {
                admin.BatchNumber = "";
            }

            // Timestamps
            admin.CreatedUtc = DateTime.UtcNow;
            admin.UpdatedUtc = DateTime.UtcNow;

            _context.Administrations.Add(admin);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        // ==========================
        // EDIT (GET - Partial)
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var admin = await _context.Administrations
                .Include(a => a.Patient)
                .Include(a => a.Batch)
                .FirstOrDefaultAsync(a => a.AdministrationId == id);

            if (admin == null) return NotFound();

            var dto = _mapper.Map<AdministrationUpdateDto>(admin);
            dto.PatientName = admin.Patient != null ? $"{admin.Patient.FirstName} {admin.Patient.LastName}" : "";

            await PopulateDropdownsAsync();

            return PartialView("_EditAdministrationPartial", dto);
        }


        // ==========================
        // EDIT (POST - AJAX)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdministrationUpdateDto dto)  
        {
            // Ensure PatientName is populated
            if (dto.PatientId != 0 && string.IsNullOrEmpty(dto.PatientName))
            {
                var patient = await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PatientId == dto.PatientId);
                if (patient != null)
                {
                    dto.PatientName = $"{patient.FirstName} {patient.LastName}";
                }
            }

            // Check foreign keys
            if (!await _context.Clinicians.AnyAsync(c => c.ClinicianId == dto.ClinicianId))
                ModelState.AddModelError("ClinicianId", "Selected Clinician does not exist.");

            if (!await _context.Vaccines.AnyAsync(v => v.VaccineId == dto.VaccineId))
                ModelState.AddModelError("VaccineId", "Selected Vaccine does not exist.");

            if (!await _context.Facilities.AnyAsync(f => f.FacilityId == dto.FacilityId))
                ModelState.AddModelError("FacilityId", "Selected Facility does not exist.");

            if (dto.BatchId.HasValue &&
                !await _context.VaccineBatches.AnyAsync(b => b.BatchId == dto.BatchId.Value))
                ModelState.AddModelError("BatchId", "Selected Batch does not exist.");

            // Duplicate check for Edit (exclude current record)
            bool isDuplicate = await _context.Administrations.AnyAsync(a =>
                a.AdministrationId != dto.AdministrationId &&
                a.PatientId == dto.PatientId &&
                a.VaccineId == dto.VaccineId &&
                a.DoseNumber == dto.DoseNumber
            );

            if (isDuplicate)
            {
                TempData["AdminError"] = "This administration record already exists for this patient.";
                await PopulateDropdownsAsync();
                return PartialView("_EditAdministrationPartial", dto);
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return PartialView("_EditAdministrationPartial", dto);
            }

            try
            {
                var admin = await _context.Administrations
                    .FirstOrDefaultAsync(a => a.AdministrationId == dto.AdministrationId);

                if (admin == null)
                    return Json(new { success = false, message = "Record not found." });

                // Update entity
                admin.VaccineId = dto.VaccineId;
                admin.BatchId = dto.BatchId;
                admin.DoseNumber = dto.DoseNumber;
                admin.AdministeredOn = dto.AdministeredOn;
                admin.ClinicianId = dto.ClinicianId;
                admin.FacilityId = dto.FacilityId;
                admin.Notes = dto.Notes;

                // Update BatchNumber
                if (dto.BatchId.HasValue)
                {
                    var batch = await _context.VaccineBatches
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.BatchId == dto.BatchId.Value);
                    admin.BatchNumber = batch != null ? batch.BatchNumber : "";
                }

                admin.UpdatedUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { success = true, updatedId = admin.AdministrationId });
            }
            catch (Exception ex)
            {
                // Fallback: unlikely to hit now because we pre-check duplicates
                TempData["AdminError"] = "An unexpected error occurred while updating: " + (ex.InnerException?.Message ?? ex.Message);
                await PopulateDropdownsAsync();
                return PartialView("_EditAdministrationPartial", dto);
            }
        }


        // =========================================================
        // HELPERS
        // =========================================================
        private async Task PopulateDropdownsAsync()
        {
            // ======================
            // Batches
            // ======================
            var batches = await _context.VaccineBatches
                .Where(b => b.ExpiryDate >= DateTime.Today)
                .Select(b => new { b.BatchId, b.BatchNumber, b.VaccineId })
                .ToListAsync();

            ViewBag.Batches = new SelectList(
                batches.Count > 0 ? batches : new List<dynamic>(),
                "BatchId",   // value field
                "BatchNumber" // display text
            );
             

            // ======================
            // Vaccines
            // ======================
            var vaccines = await _context.Vaccines
                .Select(v => new { v.VaccineId, v.Name })
                .ToListAsync();

            ViewBag.Vaccines = new SelectList(
                vaccines.Count > 0 ? vaccines : new List<dynamic>(),
                "VaccineId",
                "Name"
            );

            // ======================
            // Clinicians
            // ======================
            var clinicians = await _context.Clinicians
                .Select(c => new { c.ClinicianId, FullName = c.FirstName + " " + c.LastName })
                .ToListAsync();

            ViewBag.Clinicians = new SelectList(
                clinicians.Count > 0 ? clinicians : new List<dynamic>(),
                "ClinicianId",
                "FullName"
            );

            // ======================
            // Facilities
            // ======================
            var facilities = await _context.Facilities
                .Select(f => new { f.FacilityId, f.FacilityName })
                .ToListAsync();

            ViewBag.Facilities = new SelectList(
                facilities.Count > 0 ? facilities : new List<dynamic>(),
                "FacilityId",
                "FacilityName"
            );
        }

        [HttpGet]
        public async Task<IActionResult> SearchPatients(string term = "", int page = 1, int pageSize = 5)
        {
            term = term?.Trim().ToLower() ?? "";

            // Base query
            var query = _context.Patients
                .AsNoTracking()
                .Where(p => string.IsNullOrEmpty(term)
                            || EF.Functions.Like(p.FirstName.ToLower(), $"%{term}%")
                            || EF.Functions.Like(p.LastName.ToLower(), $"%{term}%"));

            // Count total records (for pagination)
            var totalRecords = await query.CountAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Make sure current page is not beyond totalPages
            if (page > totalPages && totalPages > 0)
                page = totalPages;
            if (page < 1)
                page = 1;

            // Get paginated results
            var patients = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination info to View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = term;

            return PartialView("_PatientSearchResults", patients);
        }

        [HttpGet]
        public async Task<IActionResult> GetBatchesByVaccine(int vaccineId)
        {
            var batches = await _context.VaccineBatches
                .Where(b => b.VaccineId == vaccineId &&
                            b.ExpiryDate >= DateTime.Today)
                .Select(b => new
                {
                    b.BatchId,
                    b.BatchNumber
                })
                .ToListAsync();

            return Json(batches);
        }

        [HttpGet]
        public async Task<IActionResult> GetNextDose(int patientId, int vaccineId)
        {
            var vaccine = await _context.Vaccines
                .FirstOrDefaultAsync(v => v.VaccineId == vaccineId);

            if (vaccine == null)
                return Json(new { success = false });

            var lastDose = await _context.Administrations
                .Where(a => a.PatientId == patientId &&
                            a.VaccineId == vaccineId)
                .MaxAsync(a => (int?)a.DoseNumber) ?? 0;

            var nextDose = lastDose + 1;

            if (nextDose > vaccine.DosesRequired)
            {
                return Json(new
                {
                    success = false,
                    message = "All required doses already administered."
                });
            }

            return Json(new
            {
                success = true,
                nextDose,
                maxDose = vaccine.DosesRequired
            });
        }


    }
}
