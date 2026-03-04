using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Vaccine;
using SyntroVaccPApp.Infrastructure;
using SyntroVaccPApp.Models; 

namespace SyntroVaccPApp.Controllers
{
    public class VaccineController(SyntroVaccPAppDbContext context, IMapper mapper) : Controller
    {
        private readonly SyntroVaccPAppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        // GET: /Vaccine
        [Authorize(Roles = "Admin,Clinician,Clerk,Auditor")]
        public async Task<IActionResult> Index(int? page, int pageSize = 5)
        {
            int pageNumber = page ?? 1;
             
            var totalItems = await _context.Vaccines.CountAsync();
             
            var vaccines = await _context.Vaccines
                .OrderBy(v => v.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
 
            var dtoList = _mapper.Map<List<VaccineReadDto>>(vaccines);
             
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PageSize = pageSize;

            return View(dtoList);
        }

        // GET: /Vaccine/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var dto = new VaccineCreateDto();
             
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_CreateVaccinePartial", dto);
            }

            return View(dto);
        }

        // POST: /Vaccine/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VaccineCreateDto dto)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto);

            var vaccine = _mapper.Map<Vaccine>(dto);
            _context.Vaccines.Add(vaccine);

            await _context.SaveChangesAsync();

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Vaccine),
                EntityId = vaccine.VaccineId,
                Action = "Create",
                ActionDate = DateTime.UtcNow,
                ChangedBy = CurrentUser.UserName
            });
             
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vaccination record created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Vaccine/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var vaccine = await _context.Vaccines.FindAsync(id);
            if (vaccine == null) return NotFound();

            var dto = _mapper.Map<VaccineUpdateDto>(vaccine);
             
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_EditVaccinePartial", dto);
            }

            return View(dto);
        }

        // POST: /Vaccine/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VaccineUpdateDto dto)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            if (!ModelState.IsValid)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    return PartialView("_EditVaccinePartial", dto);
                return View(dto);
            }

            var vaccine = await _context.Vaccines.FindAsync(id);
            if (vaccine == null) return NotFound();

            _mapper.Map(dto, vaccine);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Vaccine),
                EntityId = vaccine.VaccineId,
                Action = "Update",
                ActionDate = DateTime.UtcNow,
                ChangedBy = CurrentUser.UserName,
                ChangeSummary = $"Updated vaccine definition: {vaccine.Name}"  
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vaccine updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Vaccine/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var vaccine = await _context.Vaccines.FindAsync(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        // POST: /Vaccine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var vaccine = await _context.Vaccines.FindAsync(id);
            if (vaccine == null) return NotFound();

            _context.Vaccines.Remove(vaccine);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Vaccine),
                EntityId = vaccine.VaccineId,
                Action = "Delete",
                ActionDate = DateTime.UtcNow,
                ChangedBy = CurrentUser.UserName
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
