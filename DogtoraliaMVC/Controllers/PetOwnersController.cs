using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers;

public class PetOwnersController : Controller
{
    private readonly DogtoraliaDbContext _context;

    public PetOwnersController(DogtoraliaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var owners = await _context.PetOwners
            .OrderBy(o => o.Name)
            .Include(o => o.Pets)
            .ToListAsync();

        return View(owners);
    }

    public async Task<IActionResult> Details(int id)
    {
        var owner = await _context.PetOwners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner == null) return NotFound();

        var vm = new PetOwnerDetailsViewModel
        {
            Owner = owner,
            Pets = owner.Pets.OrderBy(p => p.Name).ToList()
        };

        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new PetOwnerFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetOwnerFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _context.PetOwners.AnyAsync(o => o.Email == vm.Email))
        {
            ModelState.AddModelError(nameof(vm.Email), "Ya existe un propietario con este correo electrónico.");
            return View(vm);
        }

        var owner = new PetOwner
        {
            Name = vm.Name,
            Email = vm.Email,
            Phone = vm.Phone,
            CreatedAt = DateTime.UtcNow
        };

        _context.PetOwners.Add(owner);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var owner = await _context.PetOwners.FindAsync(id);
        if (owner == null) return NotFound();

        var vm = new PetOwnerFormViewModel
        {
            Id = owner.Id,
            Name = owner.Name,
            Email = owner.Email,
            Phone = owner.Phone
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PetOwnerFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
            return View(vm);

        if (await _context.PetOwners.AnyAsync(o => o.Email == vm.Email && o.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Email), "Ya existe otro propietario con este correo electrónico.");
            return View(vm);
        }

        var owner = await _context.PetOwners.FindAsync(id);
        if (owner == null) return NotFound();

        owner.Name = vm.Name;
        owner.Email = vm.Email;
        owner.Phone = vm.Phone;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.PetOwners.AnyAsync(o => o.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var owner = await _context.PetOwners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner == null) return NotFound();
        return View(owner);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var owner = await _context.PetOwners
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (owner == null)
            return RedirectToAction(nameof(Index));

        if (owner.Pets.Any())
        {
            ModelState.AddModelError(string.Empty, "No se puede eliminar el propietario porque tiene mascotas asociadas.");
            return View("Delete", owner);
        }

        try
        {
            _context.PetOwners.Remove(owner);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo eliminar el propietario. Inténtelo de nuevo.");
            return View("Delete", owner);
        }

        return RedirectToAction(nameof(Index));
    }
}
