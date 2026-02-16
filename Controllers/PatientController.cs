using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Patient;
using SyntroVaccPApp.Infrastructure;
using SyntroVaccPApp.Models;
 

namespace SyntroVaccPApp.Controllers
{
    public class PatientController(SyntroVaccPAppDbContext context, IMapper mapper) : Controller
    {
        private readonly SyntroVaccPAppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        // -------------------------
        // GET: Patient
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            if (pageSize != 5 && pageSize != 10 && pageSize != 20)
            {
                pageSize = 5;  
            }

            var totalPatients = await _context.Patients.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPatients / (double)pageSize);

          
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var patients = await _context.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

  
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(patients);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (CurrentUser.Role != AppRoles.Admin) return Forbid();

            // Populate countries for the dropdown in the partial
            ViewBag.Countries = await _context.Countries.OrderBy(c => c.CountryName).ToListAsync();

            var dto = new PatientCreateDto();

            // FIX: If the request is from the Modal, return the Partial View
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_CreatePatientPartial", dto);
            }

            return PartialView("_CreatePatientPartial", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientCreateDto dto)
        {
            if (CurrentUser.Role != AppRoles.Admin) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await _context.Countries.OrderBy(c => c.CountryName).ToListAsync();

                // Return partial if validation fails in a modal
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    return PartialView("_CreatePatientPartial", dto);

                return PartialView("_CreatePatientPartial", dto);
            }

            var patient = _mapper.Map<Patient>(dto);
            var currentUser = User.Identity?.Name ?? "System";

            patient.CreatedDate = DateTime.UtcNow;
            patient.CreatedBy = currentUser;
            patient.IsActive = true;

            if (!string.IsNullOrWhiteSpace(dto.IdentifierValue))
            {
                // Check if Id number / passport is already used
                bool duplicate = await _context.PatientIdentifiers
                    .AnyAsync(pi => pi.IdentifierValue == dto.IdentifierValue);

                if (duplicate)
                {
                    ModelState.AddModelError("IdentifierValue",
                        "This patient already exists. please check id number.");

                    return PartialView("_CreatePatientIdentifierPartial", dto);
                }  

                patient.PatientIdentifiers = new List<PatientIdentifier>
                    {
                        new PatientIdentifier
                        {
                            IdentifierType = dto.IdentifierType!.Trim(),
                            IdentifierValue = dto.IdentifierValue!.Trim(),
                            CountryCode = dto.CountryCode!.Trim().ToUpper(),
                            IsPrimary = true,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = currentUser
                        }
                    };
            }

            _context.Patients.Add(patient);
 
            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Patient",
                Action = "CREATE",
                ActionDate = DateTime.UtcNow,
                ChangedBy = currentUser,
                ChangeSummary = $"Created record for {patient.FirstName} {patient.LastName}"
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Patient record created successfully!";
            return RedirectToAction(nameof(Index));
        } 
         

        [HttpGet]
        public async Task<IActionResult> Details(int id, int page = 1, int pageSize = 5)
        {
            if (pageSize != 5 && pageSize != 10 && pageSize != 20)
            {
                pageSize = 5;
            }

            var patient = await _context.Patients
                .Include(p => p.PatientIdentifiers)
                .Include(p => p.Administrations!)
                .ThenInclude(a => a.Vaccine)
                .Include(p => p.Administrations!)
                .ThenInclude(a => a.Clinician)
                .Include(p => p.Administrations!)
                .ThenInclude(a => a.Facility)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();
            
            var totalAdmin = patient.Administrations?.Count ?? 0;
            var totalPages = (int)Math.Ceiling(totalAdmin / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pagedAdmin = patient.Administrations
                ?.OrderBy(a => a.AdministeredOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Administration>();

            var viewModel = new PatientDetailsViewModel
            {
                Patient = patient,
                Administrations = pagedAdmin,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return PartialView("_PatientDetailsPartial", viewModel);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(viewModel);
        }

        // -------------------------
        // GET: /Patient/Edit/5
        // -------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        { 
            var patient = await _context.Patients
                .Include(p => p.PatientIdentifiers)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();

            ViewBag.Countries = await _context.Countries.ToListAsync();
             
            var dto = _mapper.Map<PatientUpdateDto>(patient);

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_EditPatientPartial", dto);
            }

            return View(dto);
        } 

        // -------------------------
        // POST: /Patient/Edit/5
        // -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientUpdateDto dto)
        {
            if (CurrentUser.Role == AppRoles.Auditor)
                return Forbid();
             
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await _context.Countries.OrderBy(c => c.CountryName).ToListAsync();

                // Return partial if validation fails in a modal
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    return PartialView("_EditPatientPartial", dto);

                return PartialView("_EditPatientPartial", dto);
            }

            var patient = await _context.Patients
                .Include(p => p.PatientIdentifiers)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return NotFound();
             
            _mapper.Map(dto, patient);
             
            var identifier = patient.PatientIdentifiers?.FirstOrDefault(i => i.IsPrimary);
            var currentUser = User.Identity?.Name ?? "System";

            if (!string.IsNullOrWhiteSpace(dto.IdentifierValue))
            {
                // Check if ID/passport is already used by **another patient**
                bool duplicate = await _context.PatientIdentifiers
                    .AnyAsync(pi => pi.IdentifierValue == dto.IdentifierValue
                                    && pi.PatientId != patient.PatientId);

                if (duplicate)
                {
                    ModelState.AddModelError("IdentifierValue",
                        "This patient already exists. Please check ID/Passport number.");

                    ViewBag.Countries = await _context.Countries.OrderBy(c => c.CountryName).ToListAsync();
                    return PartialView("_EditPatientPartial", dto);
                }
                 
                if (identifier == null)
                {
                    // Add new primary identifier
                    var newIdentifier = new PatientIdentifier
                    {
                        IdentifierType = dto.IdentifierType!,
                        IdentifierValue = dto.IdentifierValue!,
                        CountryCode = dto.CountryCode!,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = currentUser,
                        ModifiedDate = DateTime.UtcNow,
                        IsPrimary = true
                    };
                    patient.PatientIdentifiers ??= new List<PatientIdentifier>();
                    patient.PatientIdentifiers.Add(newIdentifier);
                }
                else
                {
                    // Update existing primary identifier
                    identifier.IdentifierType = dto.IdentifierType!;
                    identifier.IdentifierValue = dto.IdentifierValue!;
                    identifier.CountryCode = dto.CountryCode!;
                    identifier.ModifiedBy = currentUser;
                    identifier.ModifiedDate = DateTime.UtcNow;
                }
            }


            // 3. Update Patient Audit Fields
            patient.ModifiedDate = DateTime.UtcNow;
            patient.ModifiedBy = currentUser;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Record updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return PartialView("_PatientSearchResults", new List<Patient>());

            var patients = await _context.Patients
               .Include(p => p.PatientIdentifiers)
               .Where(p =>
                      p.FirstName.Contains(term) ||
                      p.LastName.Contains(term) ||
                      (p.PatientIdentifiers != null && 
                       p.PatientIdentifiers.Any(i => i.IdentifierValue != null && i.IdentifierValue.Contains(term))))
                .OrderBy(p => p.LastName)
                .Take(10)
                .ToListAsync();

            return PartialView("_PatientSearchResults", patients);
        }
         

        // -------------------------
        // GET: /Patient/Delete/5
        // -------------------------
        public async Task<IActionResult> Delete(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // -------------------------
        // POST: /Patient/Delete/5
        // -------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var patient = await _context.Patients
                .Include(p => p.PatientIdentifiers)
                .Include(p => p.Administrations)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return NotFound();

            // Remove related PatientIdentifiers
            if (patient.PatientIdentifiers != null && patient.PatientIdentifiers.Any())
                _context.PatientIdentifiers.RemoveRange(patient.PatientIdentifiers);

            // Remove related Administrations
            if (patient.Administrations != null && patient.Administrations.Any())
                _context.Administrations.RemoveRange(patient.Administrations);

            // Remove the patient
            _context.Patients.Remove(patient);

            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Patient),
                EntityId = patient.PatientId,
                Action = "Delete",
                ActionDate = DateTime.UtcNow,
                ChangedBy = User.Identity?.Name ?? "Unknown System User"
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
