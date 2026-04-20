using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers.Api.Controllers
{
    /// <summary>
    /// API controller for managing appointments.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly DogtoraliaDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentsApiController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public AppointmentsApiController(DogtoraliaDbContext db)
        {
            _db = db;
        }

        private static AppointmentDto ToDto(Appointment a) => new()
        {
            Id = a.Id,
            ClinicId = a.ClinicId,
            ClinicName = a.Clinic?.Name ?? string.Empty,
            PetId = a.PetId,
            PetName = a.Pet?.Name ?? string.Empty,
            VeterinarianId = a.VeterinarianId,
            VeterinarianFullName = a.Veterinarian?.FullName,
            AppointmentDate = a.AppointmentDate,
            Reason = a.Reason,
            Notes = a.Notes,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        };

        /// <summary>
        /// Gets all appointments, with optional filters by clinic and status.
        /// </summary>
        /// <param name="clinicId">Optional clinic identifier to filter by.</param>
        /// <param name="status">Optional appointment status to filter by.</param>
        /// <returns>A filtered list of appointments.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? clinicId, [FromQuery] AppointmentStatus? status)
        {
            var query = _db.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .AsQueryable();

            if (clinicId.HasValue)
                query = query.Where(a => a.ClinicId == clinicId.Value);

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            var appointments = await query.ToListAsync();
            return Ok(appointments.Select(ToDto));
        }

        /// <summary>
        /// Gets an appointment by its identifier.
        /// </summary>
        /// <param name="id">The appointment identifier.</param>
        /// <returns>The appointment, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();
            return Ok(ToDto(appointment));
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="dto">The appointment data.</param>
        /// <returns>The created appointment with its assigned identifier.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clinicExists = await _db.Clinics.AnyAsync(c => c.Id == dto.ClinicId);
            if (!clinicExists)
            {
                ModelState.AddModelError(nameof(dto.ClinicId), "Clinic not found.");
                return BadRequest(ModelState);
            }

            var petExists = await _db.Pets.AnyAsync(p => p.Id == dto.PetId);
            if (!petExists)
            {
                ModelState.AddModelError(nameof(dto.PetId), "Pet not found.");
                return BadRequest(ModelState);
            }

            if (dto.VeterinarianId.HasValue)
            {
                var vetExists = await _db.Veterinarians.AnyAsync(v => v.Id == dto.VeterinarianId.Value);
                if (!vetExists)
                {
                    ModelState.AddModelError(nameof(dto.VeterinarianId), "Veterinarian not found.");
                    return BadRequest(ModelState);
                }
            }

            var appointment = new Appointment
            {
                ClinicId = dto.ClinicId,
                PetId = dto.PetId,
                VeterinarianId = dto.VeterinarianId,
                AppointmentDate = dto.AppointmentDate,
                Reason = dto.Reason,
                Notes = dto.Notes,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            await _db.Entry(appointment).Reference(a => a.Clinic).LoadAsync();
            await _db.Entry(appointment).Reference(a => a.Pet).LoadAsync();
            if (appointment.VeterinarianId.HasValue)
                await _db.Entry(appointment).Reference(a => a.Veterinarian).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, ToDto(appointment));
        }

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <param name="id">The appointment identifier.</param>
        /// <param name="dto">The updated appointment data.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AppointmentWriteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var clinicExists = await _db.Clinics.AnyAsync(c => c.Id == dto.ClinicId);
            if (!clinicExists)
            {
                ModelState.AddModelError(nameof(dto.ClinicId), "Clinic not found.");
                return BadRequest(ModelState);
            }

            var petExists = await _db.Pets.AnyAsync(p => p.Id == dto.PetId);
            if (!petExists)
            {
                ModelState.AddModelError(nameof(dto.PetId), "Pet not found.");
                return BadRequest(ModelState);
            }

            if (dto.VeterinarianId.HasValue)
            {
                var vetExists = await _db.Veterinarians.AnyAsync(v => v.Id == dto.VeterinarianId.Value);
                if (!vetExists)
                {
                    ModelState.AddModelError(nameof(dto.VeterinarianId), "Veterinarian not found.");
                    return BadRequest(ModelState);
                }
            }

            appointment.ClinicId = dto.ClinicId;
            appointment.PetId = dto.PetId;
            appointment.VeterinarianId = dto.VeterinarianId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.Reason = dto.Reason;
            appointment.Notes = dto.Notes;
            appointment.Status = dto.Status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes an appointment by its identifier.
        /// </summary>
        /// <param name="id">The appointment identifier.</param>
        /// <returns>204 No Content on success, or 404 if not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
