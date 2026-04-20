using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing veterinary clinics.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClinicsApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public ClinicsApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        private static ClinicDto ToDto(Clinic c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Phone = c.Phone,
            Email = c.Email,
            Website = c.Website,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            SpecialityId = c.SpecialityId,
            SpecialityName = c.Speciality?.Name ?? string.Empty
        };

        /// <summary>
        /// Gets all clinics.
        /// </summary>
        /// <returns>A list of all clinics.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clinics = await _db.Clinics.Include(c => c.Speciality).ToListAsync();
            return Ok(clinics.Select(ToDto));
        }

        /// <summary>
        /// Gets a clinic by its identifier.
        /// </summary>
        /// <param name="id">The clinic identifier.</param>
        /// <returns>The clinic, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var clinic = await _db.Clinics.Include(c => c.Speciality).FirstOrDefaultAsync(c => c.Id == id);
            if (clinic == null) return NotFound();
            return Ok(ToDto(clinic));
        }

        /// <summary>
        /// Creates a new clinic.
        /// </summary>
        /// <param name="dto">The clinic data.</param>
        /// <returns>The created clinic with its assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ClinicWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var specialityExists = await _db.Specialities.AnyAsync(s => s.Id == dto.SpecialityId);
            if (!specialityExists)
            {
                ModelState.AddModelError(nameof(dto.SpecialityId), "Speciality not found.");
                return BadRequest(ModelState);
            }

            var clinic = new Clinic
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                Website = dto.Website,
                Description = dto.Description,
                SpecialityId = dto.SpecialityId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Clinics.Add(clinic);
            await _db.SaveChangesAsync();

            await _db.Entry(clinic).Reference(c => c.Speciality).LoadAsync();
            return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, ToDto(clinic));
        }

        /// <summary>
        /// Updates an existing clinic.
        /// </summary>
        /// <param name="id">The clinic identifier.</param>
        /// <param name="dto">The updated clinic data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClinicWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clinic = await _db.Clinics.FindAsync(id);
            if (clinic == null) return NotFound();

            var specialityExists = await _db.Specialities.AnyAsync(s => s.Id == dto.SpecialityId);
            if (!specialityExists)
            {
                ModelState.AddModelError(nameof(dto.SpecialityId), "Speciality not found.");
                return BadRequest(ModelState);
            }

            clinic.Name = dto.Name;
            clinic.Address = dto.Address;
            clinic.Phone = dto.Phone;
            clinic.Email = dto.Email;
            clinic.Website = dto.Website;
            clinic.Description = dto.Description;
            clinic.SpecialityId = dto.SpecialityId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a clinic by its identifier.
        /// </summary>
        /// <param name="id">The clinic identifier.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clinic = await _db.Clinics.FindAsync(id);
            if (clinic == null) return NotFound();

            _db.Clinics.Remove(clinic);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
