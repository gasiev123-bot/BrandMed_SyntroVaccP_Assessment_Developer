using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers
{
    public class AuditLogController(SyntroVaccPAppDbContext context) : Controller
    {
        private readonly SyntroVaccPAppDbContext _context = context;

        // GET: /AuditLog
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var totalItems = await _context.AuditLogs.CountAsync();

            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.ActionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(logs);
        }
    }
 
}
