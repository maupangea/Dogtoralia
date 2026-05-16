using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly DogtoraliaContext _db;

    public AppointmentsController(DogtoraliaContext db) => _db = db;

    private static AppointmentDto ToDto(Appointment a) => new()
    {
        Id = a.Id,
        ClinicId = a.ClinicId,
        ClinicName = a.Clinic?.Name ?? string.Empty,
        PetId = a.PetId,
        PetName = a.Pet?.Name ?? string.Empty,
        PetSpecies = a.Pet?.Species ?? string.Empty,
        PetBreed = a.Pet?.Breed ?? string.Empty,
        PetOwnerId = a.Pet?.PetOwnerId ?? 0,
        VeterinarianId = a.VeterinarianId,
        VeterinarianFullName = a.Veterinarian?.FullName,
        AppointmentDate = a.AppointmentDate,
        Reason = a.Reason,
        Notes = a.Notes,
        Status = a.Status,
        CreatedAt = a.CreatedAt
    };

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
