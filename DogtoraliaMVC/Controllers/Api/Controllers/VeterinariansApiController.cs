using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing veterinarians.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VeterinariansApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="VeterinariansApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public VeterinariansApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        private static VeterinarianDto ToDto(Veterinarian v) => new()
        {
            Id = v.Id,
            FirstName = v.FirstName,
            LastName = v.LastName,
            FullName = v.FullName,
            LicenseNumber = v.LicenseNumber,
            Email = v.Email,
            Phone = v.Phone,
            YearsOfExperience = v.YearsOfExperience,
            ClinicId = v.ClinicId,
            ClinicName = v.Clinic?.Name ?? string.Empty
        };

        /// <summary>
        /// Gets all veterinarians.
        /// </summary>
        /// <returns>A list of all veterinarians.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vets = await _db.Veterinarians.Include(v => v.Clinic).ToListAsync();
            return Ok(vets.Select(ToDto));
        }

        /// <summary>
        /// Gets a veterinarian by their identifier.
        /// </summary>
        /// <param name="id">The veterinarian identifier.</param>
        /// <returns>The veterinarian, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vet = await _db.Veterinarians.Include(v => v.Clinic).FirstOrDefaultAsync(v => v.Id == id);
            if (vet == null) return NotFound();
            return Ok(ToDto(vet));
        }

        /// <summary>
        /// Creates a new veterinarian.
        /// </summary>
        /// <param name="dto">The veterinarian data.</param>
        /// <returns>The created veterinarian with their assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(VeterinarianWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var licenseInUse = await _db.Veterinarians.AnyAsync(v => v.LicenseNumber == dto.LicenseNumber);
            if (licenseInUse)
            {
                ModelState.AddModelError(nameof(dto.LicenseNumber), "License number is already in use.");
                return BadRequest(ModelState);
            }

            var clinicExists = await _db.Clinics.AnyAsync(c => c.Id == dto.ClinicId);
            if (!clinicExists)
            {
                ModelState.AddModelError(nameof(dto.ClinicId), "Clinic not found.");
                return BadRequest(ModelState);
            }

            var vet = new Veterinarian
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                LicenseNumber = dto.LicenseNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                YearsOfExperience = dto.YearsOfExperience,
                ClinicId = dto.ClinicId
            };
            _db.Veterinarians.Add(vet);
            await _db.SaveChangesAsync();

            await _db.Entry(vet).Reference(v => v.Clinic).LoadAsync();
            return CreatedAtAction(nameof(GetById), new { id = vet.Id }, ToDto(vet));
        }

        /// <summary>
        /// Updates an existing veterinarian.
        /// </summary>
        /// <param name="id">The veterinarian identifier.</param>
        /// <param name="dto">The updated veterinarian data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VeterinarianWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vet = await _db.Veterinarians.FindAsync(id);
            if (vet == null) return NotFound();

            var licenseInUse = await _db.Veterinarians.AnyAsync(v => v.LicenseNumber == dto.LicenseNumber && v.Id != id);
            if (licenseInUse)
            {
                ModelState.AddModelError(nameof(dto.LicenseNumber), "License number is already in use.");
                return BadRequest(ModelState);
            }

            var clinicExists = await _db.Clinics.AnyAsync(c => c.Id == dto.ClinicId);
            if (!clinicExists)
            {
                ModelState.AddModelError(nameof(dto.ClinicId), "Clinic not found.");
                return BadRequest(ModelState);
            }

            vet.FirstName = dto.FirstName;
            vet.LastName = dto.LastName;
            vet.LicenseNumber = dto.LicenseNumber;
            vet.Email = dto.Email;
            vet.Phone = dto.Phone;
            vet.YearsOfExperience = dto.YearsOfExperience;
            vet.ClinicId = dto.ClinicId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a veterinarian by their identifier.
        /// </summary>
        /// <param name="id">The veterinarian identifier.</param>
        /// <returns>204 No Content on success, 404 if not found, or 409 if appointments are still linked.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vet = await _db.Veterinarians.FindAsync(id);
            if (vet == null) return NotFound();

            try
            {
                _db.Veterinarians.Remove(vet);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Cannot delete a veterinarian who has appointments associated with them." });
            }
        }
    }
}
