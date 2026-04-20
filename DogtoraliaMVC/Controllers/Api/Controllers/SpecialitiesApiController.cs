using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing veterinary specialities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialitiesApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialitiesApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public SpecialitiesApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all specialities.
        /// </summary>
        /// <returns>A list of all specialities.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specialities = await _db.Specialities
                .Select(s => new SpecialityDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
            return Ok(specialities);
        }

        /// <summary>
        /// Gets a speciality by its identifier.
        /// </summary>
        /// <param name="id">The speciality identifier.</param>
        /// <returns>The speciality, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _db.Specialities.FindAsync(id);
            if (s == null) return NotFound();
            return Ok(new SpecialityDto { Id = s.Id, Name = s.Name });
        }

        /// <summary>
        /// Creates a new speciality.
        /// </summary>
        /// <param name="dto">The speciality data.</param>
        /// <returns>The created speciality with its assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(SpecialityWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var speciality = new Speciality { Name = dto.Name };
            _db.Specialities.Add(speciality);
            await _db.SaveChangesAsync();

            var result = new SpecialityDto { Id = speciality.Id, Name = speciality.Name };
            return CreatedAtAction(nameof(GetById), new { id = speciality.Id }, result);
        }

        /// <summary>
        /// Updates an existing speciality.
        /// </summary>
        /// <param name="id">The speciality identifier.</param>
        /// <param name="dto">The updated speciality data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpecialityWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var speciality = await _db.Specialities.FindAsync(id);
            if (speciality == null) return NotFound();

            speciality.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a speciality by its identifier.
        /// </summary>
        /// <param name="id">The speciality identifier.</param>
        /// <returns>204 No Content on success, 404 if not found, or 409 if clinics are still linked.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var speciality = await _db.Specialities.FindAsync(id);
            if (speciality == null) return NotFound();

            try
            {
                _db.Specialities.Remove(speciality);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Cannot delete a speciality that has clinics associated with it." });
            }
        }
    }
}
