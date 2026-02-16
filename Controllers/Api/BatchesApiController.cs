using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers.Api
{
    [Route("api/batches")]
    [ApiController]
    public class BatchesApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public BatchesApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        // GET: api/batches?vaccineId=1
        [HttpGet]
        public async Task<IActionResult> GetBatches([FromQuery] int? vaccineId)
        {
            var query = _context.VaccineBatches
                .Include(b => b.Vaccine)
                .AsQueryable();

            if (vaccineId.HasValue)
            {
                query = query.Where(b => b.VaccineId == vaccineId.Value);
            }

            var batches = await query
                .Select(b => new
                {
                    b.BatchId,
                    b.BatchNumber,
                    b.ExpiryDate,
                    b.VaccineId,
                    VaccineName = b.Vaccine!.Name
                })
                .ToListAsync();

            return Ok(batches);
        }

        // POST: api/batches
        [HttpPost]
        public async Task<IActionResult> CreateBatch([FromBody] VaccineBatch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate vaccine exists
            var vaccineExists = await _context.Vaccines
                .AnyAsync(v => v.VaccineId == model.VaccineId);

            if (!vaccineExists)
                return BadRequest("Invalid VaccineId.");

            _context.VaccineBatches.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBatches), new { id = model.BatchId }, model);
        }
    }
}
