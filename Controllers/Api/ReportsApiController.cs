using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.DTOs.Reports;


namespace SyntroVaccPApp.Controllers.Api
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public ReportsApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueVaccinations([FromQuery] DateTime asOf)
        {
            if (asOf == default)
                return BadRequest(new { message = "asOf query parameter is required (YYYY-MM-DD)." });

            var overdue = await _context.Administrations
                .Where(a =>
                    a.DoseNumber < a.Vaccine!.DosesRequired &&
                    a.Vaccine.DoseIntervalDays.HasValue)
                .Select(a => new
                {
                    a.PatientId,
                    PatientName = a.Patient!.FirstName + " " + a.Patient.LastName,
                    VaccineName = a.Vaccine!.Name,
                    a.DoseNumber,
                    a.AdministeredOn,
                    NextDoseDueDate = a.AdministeredOn
                        .AddDays(a.Vaccine.DoseIntervalDays!.Value)
                })
                .Where(x => x.NextDoseDueDate < asOf)
                .Select(x => new OverdueVaccinationDto
                {
                    PatientId = x.PatientId,
                    PatientName = x.PatientName,
                    VaccineName = x.VaccineName,
                    DoseNumber = x.DoseNumber,
                    LastDoseDate = x.AdministeredOn,
                    NextDoseDueDate = x.NextDoseDueDate,
                    DaysOverdue = (asOf - x.NextDoseDueDate).Days
                })
                .OrderByDescending(x => x.DaysOverdue)
                .ToListAsync();

            return Ok(overdue);
        }


        [HttpGet("coverage")]
        public async Task<IActionResult> GetCoverageReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest(new
                {
                    message = "From date cannot be later than To date."
                });

            var coverage = await _context.Administrations
                .Where(a => a.AdministeredOn >= from && a.AdministeredOn <= to)
                .GroupBy(a => a.Vaccine!.Name)
                .Select(g => new CoverageReportDto
                {
                    VaccineName = g.Key,
                    TotalAdministrations = g.Count()
                })
                .OrderByDescending(x => x.TotalAdministrations)
                .ToListAsync();

            return Ok(coverage);
        }

    }
}