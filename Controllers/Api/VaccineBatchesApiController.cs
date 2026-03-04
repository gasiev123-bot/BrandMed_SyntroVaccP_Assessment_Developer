using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers.Api
{
    [ApiController]
    [Route("api/vaccine-batches")]
    public class VaccineBatchesApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public VaccineBatchesApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        // GET: api/vaccine-batches
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var batches = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .Select(b => new
                {
                    b.BatchId,
                    b.BatchNumber,
                    VaccineName = b.Vaccine!.Name,
                    b.ExpiryDate 
                })
                .ToListAsync();

            return Ok(batches);
        }

        // GET: api/vaccine-batches/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _context.VaccineBatches
                .Include(b => b.Vaccine)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null) return NotFound();

            return Ok(new
            {
                batch.BatchId,
                batch.BatchNumber,
                VaccineName = batch.Vaccine!.Name,
                batch.ExpiryDate 
            });
        }

        // POST: api/vaccine-batches
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VaccineBatch dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vaccineExists = await _context.Vaccines.AnyAsync(v => v.VaccineId == dto.VaccineId);
            if (!vaccineExists)
                return BadRequest("Vaccine does not exist.");

            _context.VaccineBatches.Add(dto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = dto.BatchId }, dto);
        }

        // PUT: api/vaccine-batches/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VaccineBatch dto)
        {
            if (id != dto.BatchId)
                return BadRequest("BatchId mismatch.");

            var batch = await _context.VaccineBatches.FindAsync(id);
            if (batch == null)
                return NotFound("Batch not found.");

            var vaccineExists = await _context.Vaccines.AnyAsync(v => v.VaccineId == dto.VaccineId);
            if (!vaccineExists)
                return BadRequest("Vaccine does not exist.");

            batch.BatchNumber = dto.BatchNumber;
            batch.VaccineId = dto.VaccineId;
            batch.ExpiryDate = dto.ExpiryDate; 

            await _context.SaveChangesAsync();

            return Ok(batch);
        }

        // DELETE: api/vaccine-batches/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.VaccineBatches.FindAsync(id);
            if (batch == null)
                return NotFound();

            _context.VaccineBatches.Remove(batch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
