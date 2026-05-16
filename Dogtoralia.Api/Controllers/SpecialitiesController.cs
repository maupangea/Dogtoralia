using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecialitiesController : ControllerBase
{
    private readonly DogtoraliaContext _db;

    public SpecialitiesController(DogtoraliaContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var specialities = await _db.Specialities
            .Select(s => new SpecialityDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
        return Ok(specialities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Specialities.FindAsync(id);
        if (s == null) return NotFound();
        return Ok(new SpecialityDto { Id = s.Id, Name = s.Name });
    }

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
