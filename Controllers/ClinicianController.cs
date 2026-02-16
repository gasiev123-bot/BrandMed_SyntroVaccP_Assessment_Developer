using SyntroVaccPApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;
using SyntroVaccPApp.DTOs.Clinician;

namespace SyntroVaccPApp.Controllers
{
    public class ClinicianController(SyntroVaccPAppDbContext context, IMapper mapper) : Controller
    {
        private readonly SyntroVaccPAppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        // GET: /Clinician
        public async Task<IActionResult> Index()
        {
            var clinicians = await _context.Clinicians.ToListAsync();
            var dtoList = _mapper.Map<List<ClinicianReadDto>>(clinicians);
            return View(dtoList);
        }

        // GET: /Clinician/Create
        public IActionResult Create()
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            return View();
        }

        // POST: /Clinician/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClinicianCreateDto dto)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto);

            var clinician = _mapper.Map<Clinician>(dto);

            _context.Clinicians.Add(clinician);
            await _context.SaveChangesAsync();

            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Clinician),
                EntityId = clinician.ClinicianId,
                Action = "Create",
                NewValue = $"Created clinician {clinician.FirstName} {clinician.LastName}",
                ActionDate = DateTime.UtcNow,
                ChangedBy = CurrentUser.UserName
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Clinician/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var clinician = await _context.Clinicians.FindAsync(id);
            if (clinician == null) return NotFound();

            var dto = _mapper.Map<ClinicianUpdateDto>(clinician);
            return View(dto);
        }

        // POST: /Clinician/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClinicianUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var clinician = await _context.Clinicians.FindAsync(id);
            if (clinician == null) return NotFound();

            _mapper.Map(dto, clinician);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Clinician/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var clinician = await _context.Clinicians.FindAsync(id);
            if (clinician == null) return NotFound();

            return View(clinician);
        }

        // POST: /Clinician/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (CurrentUser.Role != AppRoles.Admin)
                return Forbid();

            var clinician = await _context.Clinicians.FindAsync(id);
            if (clinician == null) return NotFound();

            _context.Clinicians.Remove(clinician);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = nameof(Clinician),
                EntityId = clinician.ClinicianId,
                Action = "Delete",
                NewValue = $"Deleted clinician {clinician.FirstName} {clinician.LastName}",
                ActionDate = DateTime.UtcNow,
                ChangedBy = CurrentUser.UserName
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
