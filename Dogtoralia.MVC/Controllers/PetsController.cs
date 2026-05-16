using Dogtoralia.MVC.Helpers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dogtoralia.MVC.Controllers;

[Authorize(Roles = "Admin,User")]
public class PetsController : Controller
{
    private readonly IPetService _petService;
    private readonly IPetOwnerService _petOwnerService;
    private readonly UserManager<IdentityUser> _userManager;
    private const int PageSize = 8;

    public PetsController(IPetService petService, IPetOwnerService petOwnerService, UserManager<IdentityUser> userManager)
    {
        _petService = petService;
        _petOwnerService = petOwnerService;
        _userManager = userManager;
    }

    private async Task<PetOwnerDto?> GetCurrentUserOwnerAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return null;
        var owners = await _petOwnerService.GetAllAsync();
        return owners.FirstOrDefault(o => o.UserId == userId);
    }

    public async Task<IActionResult> Index(string? species, int page = 1)
    {
        var allPets = (await _petService.GetAllAsync()).ToList();

        int? currentOwnerId = null;
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner == null) return Forbid();
            currentOwnerId = currentOwner.Id;
            allPets = allPets.Where(p => p.PetOwnerId == currentOwner.Id).ToList();
        }

        if (!string.IsNullOrWhiteSpace(species))
            allPets = allPets.Where(p => p.Species == species).ToList();

        var availableSpecies = allPets.Select(p => p.Species).Distinct().OrderBy(s => s).ToList();
        var sorted = allPets.OrderBy(p => p.Name).ToList();
        var pets = PaginatedList<PetDto>.Create(sorted, page, PageSize);

        ViewBag.CurrentPage = pets.PageIndex;
        ViewBag.TotalPages = pets.TotalPages;
        ViewBag.RouteValues = new Dictionary<string, string?> { ["species"] = species };
        ViewBag.CurrentOwnerId = currentOwnerId;

        return View(new PetsIndexViewModel
        {
            Pets = pets,
            AvailableSpecies = availableSpecies,
            SelectedSpecies = species
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (pet.PetOwnerId != currentOwner?.Id) return Forbid();
        }

        return View(pet);
    }

    public async Task<IActionResult> Create(int petOwnerId)
    {
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner?.Id != petOwnerId) return Forbid();
        }

        var owner = await _petOwnerService.GetByIdAsync(petOwnerId);
        if (owner == null) return NotFound();

        return View(new PetFormViewModel
        {
            PetOwnerId = petOwnerId,
            OwnerName = owner.Name,
            OwnerEmail = owner.Email,
            OwnerPhone = owner.Phone
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetFormViewModel vm)
    {
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner?.Id != vm.PetOwnerId) return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var owner = await _petOwnerService.GetByIdAsync(vm.PetOwnerId);
            if (owner != null)
            {
                vm.OwnerName = owner.Name;
                vm.OwnerEmail = owner.Email;
                vm.OwnerPhone = owner.Phone;
            }
            return View(vm);
        }

        await _petService.CreateAsync(new PetWriteDto
        {
            Name = vm.Name,
            Species = vm.Species,
            Breed = vm.Breed,
            DateOfBirth = vm.DateOfBirth,
            Notes = vm.Notes,
            PetOwnerId = vm.PetOwnerId
        });

        return RedirectToAction("Details", "PetOwners", new { id = vm.PetOwnerId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (pet.PetOwnerId != currentOwner?.Id) return Forbid();
        }

        return View(new PetFormViewModel
        {
            Id = pet.Id,
            PetOwnerId = pet.PetOwnerId,
            OwnerName = pet.PetOwnerName,
            OwnerEmail = pet.PetOwnerEmail,
            OwnerPhone = pet.PetOwnerPhone,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            DateOfBirth = pet.DateOfBirth,
            Notes = pet.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PetFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        var pet = await _petService.GetByIdAsync(id);
        if (pet == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (pet.PetOwnerId != currentOwner?.Id) return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var owner = await _petOwnerService.GetByIdAsync(vm.PetOwnerId);
            if (owner != null)
            {
                vm.OwnerName = owner.Name;
                vm.OwnerEmail = owner.Email;
                vm.OwnerPhone = owner.Phone;
            }
            return View(vm);
        }

        var redirectOwnerId = pet.PetOwnerId;

        await _petService.UpdateAsync(id, new PetWriteDto
        {
            Name = vm.Name,
            Species = vm.Species,
            Breed = vm.Breed,
            DateOfBirth = vm.DateOfBirth,
            Notes = vm.Notes,
            PetOwnerId = vm.PetOwnerId
        });

        return RedirectToAction("Details", "PetOwners", new { id = redirectOwnerId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (pet.PetOwnerId != currentOwner?.Id) return Forbid();
        }

        return View(pet);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet != null)
        {
            if (User.IsInRole("User"))
            {
                var currentOwner = await GetCurrentUserOwnerAsync();
                if (pet.PetOwnerId != currentOwner?.Id) return Forbid();
            }

            var ownerId = pet.PetOwnerId;
            await _petService.DeleteAsync(id);
            return RedirectToAction("Details", "PetOwners", new { id = ownerId });
        }
        return RedirectToAction("Index", "PetOwners");
    }
}
