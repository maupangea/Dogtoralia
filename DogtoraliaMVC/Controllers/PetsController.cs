using DogtoraliaMVC.Data;
using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers;

public class PetsController : Controller
{
    private readonly DogtoraliaDbContext _context;
    private const int PageSize = 8;

    public PetsController(DogtoraliaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? species, int page = 1)
    {
        var query = _context.Pets.Include(p => p.PetOwner).AsQueryable();

        if (!string.IsNullOrWhiteSpace(species))
            query = query.Where(p => p.Species == species);

        var pets = await PaginatedList<Pet>.CreateAsync(query.OrderBy(p => p.Name), page, PageSize);
        var availableSpecies = await _context.Pets.Select(p => p.Species).Distinct().OrderBy(s => s).ToListAsync();

        var vm = new PetsIndexViewModel
        {
            Pets = pets,
            AvailableSpecies = availableSpecies,
            SelectedSpecies = species
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pet = await _context.Pets
            .Include(p => p.PetOwner)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null) return NotFound();
        return View(pet);
    }

    public async Task<IActionResult> Create(int petOwnerId)
    {
        var owner = await _context.PetOwners.FindAsync(petOwnerId);
        if (owner == null) return NotFound();

        var vm = new PetFormViewModel
        {
            PetOwnerId = petOwnerId,
            OwnerName = owner.Name,
            OwnerEmail = owner.Email,
            OwnerPhone = owner.Phone
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var owner = await _context.PetOwners.FindAsync(vm.PetOwnerId);
            if (owner != null)
            {
                vm.OwnerName = owner.Name;
                vm.OwnerEmail = owner.Email;
                vm.OwnerPhone = owner.Phone;
            }
            return View(vm);
        }

        var pet = new Pet
        {
            Name = vm.Name,
            Species = vm.Species,
            Breed = vm.Breed,
            DateOfBirth = vm.DateOfBirth,
            Notes = vm.Notes,
            PetOwnerId = vm.PetOwnerId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "PetOwners", new { id = vm.PetOwnerId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pet = await _context.Pets
            .Include(p => p.PetOwner)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null) return NotFound();

        var vm = new PetFormViewModel
        {
            Id = pet.Id,
            PetOwnerId = pet.PetOwnerId,
            OwnerName = pet.PetOwner.Name,
            OwnerEmail = pet.PetOwner.Email,
            OwnerPhone = pet.PetOwner.Phone,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            DateOfBirth = pet.DateOfBirth,
            Notes = pet.Notes
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PetFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            var owner = await _context.PetOwners.FindAsync(vm.PetOwnerId);
            if (owner != null)
            {
                vm.OwnerName = owner.Name;
                vm.OwnerEmail = owner.Email;
                vm.OwnerPhone = owner.Phone;
            }
            return View(vm);
        }

        var pet = await _context.Pets.FindAsync(id);
        if (pet == null) return NotFound();

        pet.Name = vm.Name;
        pet.Species = vm.Species;
        pet.Breed = vm.Breed;
        pet.DateOfBirth = vm.DateOfBirth;
        pet.Notes = vm.Notes;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Pets.AnyAsync(p => p.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction("Details", "PetOwners", new { id = vm.PetOwnerId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _context.Pets
            .Include(p => p.PetOwner)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null) return NotFound();
        return View(pet);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        if (pet != null)
        {
            var ownerId = pet.PetOwnerId;
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "PetOwners", new { id = ownerId });
        }
        return RedirectToAction("Index", "PetOwners");
    }
}
