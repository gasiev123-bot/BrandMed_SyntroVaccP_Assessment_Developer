using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.VaccineBatch;
using SyntroVaccPApp.Infrastructure;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers
{
    [Authorize(Roles = "Admin,Clerk")]
    public class VaccineBatchController : Controller
    { 
        private readonly SyntroVaccPAppDbContext _context;
        private readonly IMapper _mapper;

        public VaccineBatchController(SyntroVaccPAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: /VaccineBatch
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            if (pageSize != 5 && pageSize != 10 && pageSize != 20)
            {
                pageSize = 5;
            }

            var totalBatches = await _context.VaccineBatches.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBatches / (double)pageSize);

            if (totalPages == 0)
                totalPages = 1;

            if (page < 1)
                page = 1;

            if (page > totalPages)
                page = totalPages;

            var batches = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .OrderByDescending(b => b.ExpiryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var dtoList = _mapper.Map<List<VaccineBatchReadDto>>(batches);
            return View(dtoList);
        }


        // GET: /VaccineBatch/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (CurrentUser.Role == AppRoles.Auditor)
                return Forbid();

            var vaccines = await _context.Vaccines
               .Where(v => v.IsActive)
               .AsNoTracking()
               .ToListAsync();

            // Pass the list directly to the view via ViewBag
            ViewBag.Vaccines = vaccines;

            var dto = new VaccineBatchCreateDto
            {
                ExpiryDate = DateTime.Today.AddYears(1)
            };

            return PartialView("_CreateBatchPartial", dto);
        }


        // POST: /VaccineBatch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VaccineBatchCreateDto dto)
        {
            if (dto.ExpiryDate < DateTime.Today)
            {
                ModelState.AddModelError("ExpiryDate", "Cannot create a batch that is already expired.");
            }

            bool duplicate = await _context.VaccineBatches.AnyAsync(b =>
                b.VaccineId == dto.VaccineId &&
                b.BatchNumber == dto.BatchNumber);

            if (duplicate)
            {
                ModelState.AddModelError("BatchNumber",
                    "This batch number already exists for the selected vaccine.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateVaccineViewBag();
                return PartialView("_CreateBatchPartial", dto); // IMPORTANT
            }

            var batch = _mapper.Map<VaccineBatch>(dto);
            _context.VaccineBatches.Add(batch);

            await _context.SaveChangesAsync();

            return Json(new { success = true }); // IMPORTANT
        }


        // GET: /VaccineBatch/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var batch = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
                return NotFound();

            var vaccines = await _context.Vaccines
                .Where(v => v.IsActive)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Vaccines = vaccines;   // 🔥 THIS WAS MISSING

            var dto = _mapper.Map<VaccineBatchUpdateDto>(batch);
            dto.Manufacturer = batch.Vaccine?.Manufacturer ?? "";

            return PartialView("_EditBatchPartial", dto);
        }


        // POST: /VaccineBatch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VaccineBatchUpdateDto dto)
        {
            if (id != dto.BatchId)
                return BadRequest();

            bool duplicate = await _context.VaccineBatches.AnyAsync(b =>
                b.VaccineId == dto.VaccineId &&
                b.BatchNumber == dto.BatchNumber &&
                b.BatchId != id);

            if (duplicate)
            {
                ModelState.AddModelError("BatchNumber",
                    "Another batch with this number already exists for the selected vaccine.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateVaccineViewBag();
                return PartialView("_EditBatchPartial", dto);
            }

            var batch = await _context.VaccineBatches.FindAsync(id);
            if (batch == null)
                return NotFound();

            _mapper.Map(dto, batch);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(VaccineBatch),
                EntityId = batch.BatchId,
                Action = "UPDATE",
                ChangedBy = User.Identity?.Name ?? "System",
                ActionDate = DateTime.UtcNow,
                ChangeSummary = $"Updated batch {batch.BatchNumber} details."
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true });   // 🔥 IMPORTANT
        }


        // POST: /VaccineBatch/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.VaccineBatches.FindAsync(id);
            if (batch == null) return NotFound();

            bool isUsed = await _context.Administrations.AnyAsync(a => a.BatchId == id);
            if (isUsed)
            {
                TempData["ErrorMessage"] = "Cannot delete a batch that has already been used for vaccinations.";
              
            }

            _context.VaccineBatches.Remove(batch);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(VaccineBatch),
                EntityId = id,
                Action = "DELETE",
                ChangedBy = User.Identity?.Name ?? "System",
                ActionDate = DateTime.UtcNow,
                ChangeSummary = $"Deleted batch {batch.BatchNumber}"
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // HELPERS
        // =========================================================
        private async Task PopulateVaccineViewBag()
        {
            var vaccines = await _context.Vaccines
                .Where(v => v.IsActive)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Vaccines = vaccines;

        }

    }
}
