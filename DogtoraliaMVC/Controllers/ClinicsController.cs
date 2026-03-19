using DogtoraliaMVC.Data;
using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers;

public class ClinicsController : Controller
{
    private readonly DogtoraliaDbContext _context;
    private const int PageSize = 6;

    public ClinicsController(DogtoraliaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? specialityId, int page = 1)
    {
        var query = _context.Clinics
            .Include(c => c.Speciality)
            .Include(c => c.Veterinarians)
            .AsQueryable();

        if (specialityId.HasValue)
            query = query.Where(c => c.SpecialityId == specialityId.Value);

        var clinics = await PaginatedList<Clinic>.CreateAsync(query.OrderBy(c => c.Name), page, PageSize);
        var specialities = await _context.Specialities.OrderBy(s => s.Name).ToListAsync();

        var vm = new ClinicsIndexViewModel
        {
            Clinics = clinics,
            Specialities = specialities,
            SelectedSpecialityId = specialityId
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Speciality)
            .Include(c => c.Veterinarians)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinic == null) return NotFound();
        return View(clinic);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ClinicFormViewModel
        {
            SpecialityOptions = new SelectList(await _context.Specialities.OrderBy(s => s.Name).ToListAsync(), "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClinicFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.SpecialityOptions = new SelectList(await _context.Specialities.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", vm.SpecialityId);
            return View(vm);
        }

        var clinic = new Clinic
        {
            Name = vm.Name,
            Address = vm.Address,
            Phone = vm.Phone,
            Email = vm.Email,
            Website = vm.Website,
            Description = vm.Description,
            SpecialityId = vm.SpecialityId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null) return NotFound();

        var vm = new ClinicFormViewModel
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Address = clinic.Address,
            Phone = clinic.Phone,
            Email = clinic.Email,
            Website = clinic.Website,
            Description = clinic.Description,
            SpecialityId = clinic.SpecialityId,
            SpecialityOptions = new SelectList(await _context.Specialities.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", clinic.SpecialityId)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClinicFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.SpecialityOptions = new SelectList(await _context.Specialities.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", vm.SpecialityId);
            return View(vm);
        }

        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null) return NotFound();

        clinic.Name = vm.Name;
        clinic.Address = vm.Address;
        clinic.Phone = vm.Phone;
        clinic.Email = vm.Email;
        clinic.Website = vm.Website;
        clinic.Description = vm.Description;
        clinic.SpecialityId = vm.SpecialityId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Clinics.AnyAsync(c => c.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Speciality)
            .Include(c => c.Veterinarians)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinic == null) return NotFound();
        return View(clinic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic != null)
        {
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
