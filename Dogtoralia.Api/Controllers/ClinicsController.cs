using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClinicsController : ControllerBase
{
    private readonly DogtoraliaContext _db;

    public ClinicsController(DogtoraliaContext db) => _db = db;

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
        SpecialityName = c.Speciality?.Name ?? string.Empty,
        VeterinarianCount = c.Veterinarians?.Count ?? 0
    };

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _db.Clinics
            .Include(c => c.Speciality)
            .Include(c => c.Veterinarians)
            .ToListAsync();
        return Ok(clinics.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var clinic = await _db.Clinics
            .Include(c => c.Speciality)
            .Include(c => c.Veterinarians)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (clinic == null) return NotFound();
        return Ok(ToDto(clinic));
    }

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
