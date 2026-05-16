using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dogtoralia.MVC.Controllers;

public class PetOwnersController : Controller
{
    private readonly IPetOwnerService _petOwnerService;
    private readonly IPetService _petService;
    private readonly UserManager<IdentityUser> _userManager;

    public PetOwnersController(IPetOwnerService petOwnerService, IPetService petService, UserManager<IdentityUser> userManager)
    {
        _petOwnerService = petOwnerService;
        _petService = petService;
        _userManager = userManager;
    }

    private async Task<PetOwnerDto?> GetCurrentUserOwnerAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return null;
        var owners = await _petOwnerService.GetAllAsync();
        return owners.FirstOrDefault(o => o.UserId == userId);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var owners = (await _petOwnerService.GetAllAsync()).OrderBy(o => o.Name).ToList();
        return View(owners);
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Details(int id)
    {
        var owner = await _petOwnerService.GetByIdAsync(id);
        if (owner == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner?.Id != id) return Forbid();
        }

        string? passwordHash = null;
        if (owner.UserId != null)
        {
            var identityUser = await _userManager.FindByIdAsync(owner.UserId);
            passwordHash = identityUser?.PasswordHash;
        }

        var allPets = await _petService.GetAllAsync();
        var pets = allPets.Where(p => p.PetOwnerId == id).OrderBy(p => p.Name).ToList();

        return View(new PetOwnerDetailsViewModel
        {
            Owner = owner,
            Pets = pets,
            PasswordHash = passwordHash,
            IsOwnProfile = User.IsInRole("User")
        });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new PetOwnerFormViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetOwnerFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var existing = await _petOwnerService.GetAllAsync();
        if (existing.Any(o => o.Email.Equals(vm.Email, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(vm.Email), "Ya existe un propietario con este correo electrónico.");
            return View(vm);
        }

        await _petOwnerService.CreateAsync(new PetOwnerWriteDto
        {
            Name = vm.Name,
            Email = vm.Email,
            Phone = vm.Phone
        });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Edit(int id)
    {
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner?.Id != id) return Forbid();
        }

        var owner = await _petOwnerService.GetByIdAsync(id);
        if (owner == null) return NotFound();

        return View(new PetOwnerFormViewModel
        {
            Id = owner.Id,
            Name = owner.Name,
            Email = owner.Email,
            Phone = owner.Phone
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PetOwnerFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner?.Id != id) return Forbid();
        }

        if (!ModelState.IsValid)
            return View(vm);

        var existing = await _petOwnerService.GetAllAsync();
        if (existing.Any(o => o.Email.Equals(vm.Email, StringComparison.OrdinalIgnoreCase) && o.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Email), "Ya existe otro propietario con este correo electrónico.");
            return View(vm);
        }

        await _petOwnerService.UpdateAsync(id, new PetOwnerWriteDto
        {
            Name = vm.Name,
            Email = vm.Email,
            Phone = vm.Phone
        });

        if (User.IsInRole("User"))
            return RedirectToAction(nameof(Details), new { id });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var owner = await _petOwnerService.GetByIdAsync(id);
        if (owner == null) return NotFound();
        return View(owner);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var owner = await _petOwnerService.GetByIdAsync(id);
        if (owner == null)
            return RedirectToAction(nameof(Index));

        if (owner.PetCount > 0)
        {
            ModelState.AddModelError(string.Empty, "No se puede eliminar el propietario porque tiene mascotas asociadas.");
            return View("Delete", owner);
        }

        var deleted = await _petOwnerService.DeleteAsync(id);
        if (!deleted)
        {
            ModelState.AddModelError(string.Empty, "No se pudo eliminar el propietario. Inténtelo de nuevo.");
            return View("Delete", owner);
        }

        return RedirectToAction(nameof(Index));
    }
}
