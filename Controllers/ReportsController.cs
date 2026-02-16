using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace SyntroVaccPApp.Controllers
{
    [Authorize(Roles = "Admin,Clerk")]
    public class ReportsController : Controller
    {
        private readonly SyntroVaccPAppDbContext _context;
        private readonly ReportService _reportService;

        public ReportsController(SyntroVaccPAppDbContext context, ReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /VaccineBatch/BatchStock
        public async Task<IActionResult> BatchStock(int page = 1, int pageSize = 5)
        {
            var allowedSizes = new[] { 5, 10, 20 };
            if (!allowedSizes.Contains(pageSize))
                pageSize = 5;

            var totalBatches = await _context.VaccineBatches.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBatches / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var batches = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .OrderByDescending(b => b.ExpiryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass stock info to ViewBag or Model (already in the model now)
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(batches);
        }


        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> OverdueList()
        {
            // Only administrations with both a patient and a vaccine
            var administrations = await _context.Administrations
                .Include(a => a.Patient)
                .Include(a => a.Vaccine)
                .Include(a => a.Facility)
                .Include(a => a.Clinician)
                .Where(a => a.Patient != null && a.Vaccine != null && a.Vaccine.DoseIntervalDays.HasValue)
                .ToListAsync();

            // Get the latest dose per patient + vaccine
            var overdueList = administrations
                .GroupBy(a => new { a.PatientId, a.VaccineId })
                .Select(g => g.OrderByDescending(x => x.DoseNumber).First())
                .Where(a => a.DoseNumber < a.Vaccine!.DosesRequired) // still requires doses
                .Select(a =>
                {
                    var intervalDays = a.Vaccine!.DoseIntervalDays!.Value;
                    var dueDate = a.AdministeredOn.AddDays(intervalDays);

                    return new OverdueReportDto
                    {
                         
                        PatientName = a.Patient != null
                            ? $"{a.Patient.FirstName} {a.Patient.LastName}"
                            : string.Empty,
                        VaccineName = a.Vaccine!.Name,
                        LastDoseNumber = a.DoseNumber,
                        DueDate = dueDate,
                        FacilityName = a.Facility?.FacilityName ?? "N/A",
                        AdministeredBy = a.Clinician != null
                            ? $"{a.Clinician.FirstName} {a.Clinician.LastName}"
                            : "N/A"
                    };
                })
                .Where(x => x.DueDate < DateTime.Today) // overdue only
                .OrderByDescending(x => x.DueDate)
                .ToList();
 

            return View(overdueList);
        }




        public async Task<IActionResult> VaccinationSummary(DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 5)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var query = _context.Administrations
                .Include(a => a.Vaccine)
                .Include(a => a.Facility)
                .Where(a => a.AdministeredOn >= start && a.AdministeredOn <= end);

            var groupedQuery = query
                .GroupBy(a => new
                {
                    VName = a.Vaccine != null ? a.Vaccine.Name : "Unknown",
                    FName = a.Facility != null ? a.Facility.FacilityName : "N/A"
                });

            var totalRecords = await groupedQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var reportData = await groupedQuery
                .OrderBy(g => g.Key.VName)
                .ThenBy(g => g.Key.FName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new VaccinationReportDto
                {
                    VaccineName = g.Key.VName ?? "Unknown",
                    FacilityName = g.Key.FName ?? "N/A",
                    TotalDoses = g.Count()
                })
                .ToListAsync();

            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(reportData);
        }


        [HttpGet]
        public async Task<IActionResult> PrintImmunisationReport(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.Administrations!)
                    .ThenInclude(a => a.Vaccine)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
                return NotFound();

            var pdf = _reportService.GenerateImmunisationReport(
                patient,
                patient.Administrations!
                    .OrderBy(a => a.AdministeredOn)
                    .ToList()
            );

            return File(pdf, "application/pdf",
                $"ImmunisationReport_{patientId}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> PrintBatchStockReport()
        {
            // Fetch all batches with related vaccine info
            var batches = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .OrderByDescending(b => b.ExpiryDate)
                .ToListAsync();

            if (batches == null || !batches.Any())
                return NotFound("No vaccine batches found.");

            // Generate PDF
            var pdfBytes = _reportService.GenerateBatchStockReport(batches);

            //   return File(pdfBytes, "application/pdf", $"VaccineBatchStock_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            return File(pdfBytes, "application/pdf");
        }

    }
}
