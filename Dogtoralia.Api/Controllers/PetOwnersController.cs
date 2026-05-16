using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PetOwnersController : ControllerBase
{
    private readonly DogtoraliaContext _db;

    public PetOwnersController(DogtoraliaContext db) => _db = db;

    private static PetOwnerDto ToDto(PetOwner o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Email = o.Email,
        Phone = o.Phone,
        CreatedAt = o.CreatedAt,
        PetCount = o.Pets?.Count ?? 0,
        UserId = o.UserId
    };

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var owners = await _db.PetOwners.Include(o => o.Pets).ToListAsync();
        return Ok(owners.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var owner = await _db.PetOwners.Include(o => o.Pets).FirstOrDefaultAsync(o => o.Id == id);
        if (owner == null) return NotFound();
        return Ok(ToDto(owner));
    }

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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PetOwnerWriteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var owner = await _db.PetOwners.Include(o => o.Pets).FirstOrDefaultAsync(o => o.Id == id);
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
