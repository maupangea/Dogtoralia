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
        var query = _context.Pets.AsQueryable();

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
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
        if (pet == null) return NotFound();
        return View(pet);
    }

    public IActionResult Create()
    {
        return View(new PetFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var pet = new Pet
        {
            Name = vm.Name,
            Species = vm.Species,
            Breed = vm.Breed,
            DateOfBirth = vm.DateOfBirth,
            OwnerName = vm.OwnerName,
            OwnerEmail = vm.OwnerEmail,
            OwnerPhone = vm.OwnerPhone,
            Notes = vm.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        if (pet == null) return NotFound();

        var vm = new PetFormViewModel
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            DateOfBirth = pet.DateOfBirth,
            OwnerName = pet.OwnerName,
            OwnerEmail = pet.OwnerEmail,
            OwnerPhone = pet.OwnerPhone,
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
            return View(vm);

        var pet = await _context.Pets.FindAsync(id);
        if (pet == null) return NotFound();

        pet.Name = vm.Name;
        pet.Species = vm.Species;
        pet.Breed = vm.Breed;
        pet.DateOfBirth = vm.DateOfBirth;
        pet.OwnerName = vm.OwnerName;
        pet.OwnerEmail = vm.OwnerEmail;
        pet.OwnerPhone = vm.OwnerPhone;
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

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
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
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
