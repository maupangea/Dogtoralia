using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing pets.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PetsApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetsApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public PetsApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        private static PetDto ToDto(Pet p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            Breed = p.Breed,
            DateOfBirth = p.DateOfBirth,
            Age = p.Age,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt,
            PetOwnerId = p.PetOwnerId,
            PetOwnerName = p.PetOwner?.Name ?? string.Empty
        };

        /// <summary>
        /// Gets all pets.
        /// </summary>
        /// <returns>A list of all pets.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _db.Pets.Include(p => p.PetOwner).ToListAsync();
            return Ok(pets.Select(ToDto));
        }

        /// <summary>
        /// Gets a pet by its identifier.
        /// </summary>
        /// <param name="id">The pet identifier.</param>
        /// <returns>The pet, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await _db.Pets.Include(p => p.PetOwner).FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();
            return Ok(ToDto(pet));
        }

        /// <summary>
        /// Creates a new pet.
        /// </summary>
        /// <param name="dto">The pet data.</param>
        /// <returns>The created pet with its assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PetWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ownerExists = await _db.PetOwners.AnyAsync(o => o.Id == dto.PetOwnerId);
            if (!ownerExists)
            {
                ModelState.AddModelError(nameof(dto.PetOwnerId), "Pet owner not found.");
                return BadRequest(ModelState);
            }

            var pet = new Pet
            {
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                DateOfBirth = dto.DateOfBirth,
                Notes = dto.Notes,
                PetOwnerId = dto.PetOwnerId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Pets.Add(pet);
            await _db.SaveChangesAsync();

            await _db.Entry(pet).Reference(p => p.PetOwner).LoadAsync();
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, ToDto(pet));
        }

        /// <summary>
        /// Updates an existing pet.
        /// </summary>
        /// <param name="id">The pet identifier.</param>
        /// <param name="dto">The updated pet data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PetWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            var ownerExists = await _db.PetOwners.AnyAsync(o => o.Id == dto.PetOwnerId);
            if (!ownerExists)
            {
                ModelState.AddModelError(nameof(dto.PetOwnerId), "Pet owner not found.");
                return BadRequest(ModelState);
            }

            pet.Name = dto.Name;
            pet.Species = dto.Species;
            pet.Breed = dto.Breed;
            pet.DateOfBirth = dto.DateOfBirth;
            pet.Notes = dto.Notes;
            pet.PetOwnerId = dto.PetOwnerId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a pet by its identifier.
        /// </summary>
        /// <param name="id">The pet identifier.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
