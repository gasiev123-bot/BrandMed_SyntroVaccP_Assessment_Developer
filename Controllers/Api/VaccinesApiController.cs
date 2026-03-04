using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Data;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Controllers.Api
{
    [ApiController]
    [Route("api/vaccines")]
    [Produces("application/json")]
    public class VaccinesApiController : ControllerBase
    {
        private readonly SyntroVaccPAppDbContext _context;

        public VaccinesApiController(SyntroVaccPAppDbContext context)
        {
            _context = context;
        }

        // GET: api/vaccines
        [HttpGet]
        public async Task<IActionResult> GetVaccines()
        {
            var vaccines = await _context.Vaccines
                .Select(v => new
                {
                    v.VaccineId,
                    v.Name,
                    v.DosesRequired,
                    v.DoseIntervalDays
                })
                .ToListAsync();

            if (!vaccines.Any())
            {
                return Ok(new
                {
                    message = "No vaccines found",
                    data = vaccines
                });
            }

            return Ok(vaccines);
        }

        // GET: api/vaccines/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vaccine = await _context.Vaccines.FindAsync(id);

            if (vaccine == null)
                return NotFound(new { message = $"Vaccine with ID {id} not found" });

            return Ok(vaccine);
        }

        // POST: api/vaccines
        [HttpPost]
        public async Task<IActionResult> CreateVaccine([FromBody] Vaccine model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    message = "Validation failed",
                    errors
                });
            }

            // Prevent duplicate vaccine names
            var exists = await _context.Vaccines
                .AnyAsync(v => v.Name == model.Name);

            if (exists)
            {
                return Conflict(new
                {
                    message = $"Vaccine '{model.Name}' already exists."
                });
            }

            // Optional business validation
            if (model.DosesRequired <= 0)
            {
                return BadRequest(new
                {
                    message = "DosesRequired must be greater than 0."
                });
            }

            _context.Vaccines.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = model.VaccineId },
                model
            );
        }
    }
}
