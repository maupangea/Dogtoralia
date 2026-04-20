using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing pet owners.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PetOwnersApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetOwnersApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public PetOwnersApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        private static PetOwnerDto ToDto(PetOwner o) => new()
        {
            Id = o.Id,
            Name = o.Name,
            Email = o.Email,
            Phone = o.Phone,
            CreatedAt = o.CreatedAt
        };

        /// <summary>
        /// Gets all pet owners.
        /// </summary>
        /// <returns>A list of all pet owners.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var owners = await _db.PetOwners.Select(o => ToDto(o)).ToListAsync();
            return Ok(owners);
        }

        /// <summary>
        /// Gets a pet owner by their identifier.
        /// </summary>
        /// <param name="id">The pet owner identifier.</param>
        /// <returns>The pet owner, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var owner = await _db.PetOwners.FindAsync(id);
            if (owner == null) return NotFound();
            return Ok(ToDto(owner));
        }

        /// <summary>
        /// Creates a new pet owner.
        /// </summary>
        /// <param name="dto">The pet owner data.</param>
        /// <returns>The created pet owner with their assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PetOwnerWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailInUse = await _db.PetOwners.AnyAsync(o => o.Email == dto.Email);
            if (emailInUse)
            {
                ModelState.AddModelError(nameof(dto.Email), "Email address is already in use.");
                return BadRequest(ModelState);
            }

            var owner = new PetOwner
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };
            _db.PetOwners.Add(owner);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = owner.Id }, ToDto(owner));
        }

        /// <summary>
        /// Updates an existing pet owner.
        /// </summary>
        /// <param name="id">The pet owner identifier.</param>
        /// <param name="dto">The updated pet owner data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PetOwnerWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var owner = await _db.PetOwners.FindAsync(id);
            if (owner == null) return NotFound();

            var emailInUse = await _db.PetOwners.AnyAsync(o => o.Email == dto.Email && o.Id != id);
            if (emailInUse)
            {
                ModelState.AddModelError(nameof(dto.Email), "Email address is already in use.");
                return BadRequest(ModelState);
            }

            owner.Name = dto.Name;
            owner.Email = dto.Email;
            owner.Phone = dto.Phone;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a pet owner by their identifier.
        /// </summary>
        /// <param name="id">The pet owner identifier.</param>
        /// <returns>204 No Content on success, 404 if not found, or 409 if pets are still linked.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var owner = await _db.PetOwners.FindAsync(id);
            if (owner == null) return NotFound();

            try
            {
                _db.PetOwners.Remove(owner);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Cannot delete a pet owner who still has pets." });
            }
        }
    }
}
