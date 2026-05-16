using Dogtoralia.MVC.Helpers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dogtoralia.MVC.Controllers;

public class ClinicsController : Controller
{
    private readonly IClinicService _clinicService;
    private readonly ISpecialityService _specialityService;
    private readonly IVeterinarianService _veterinarianService;
    private readonly IAppointmentService _appointmentService;
    private const int PageSize = 6;

    public ClinicsController(
        IClinicService clinicService,
        ISpecialityService specialityService,
        IVeterinarianService veterinarianService,
        IAppointmentService appointmentService)
    {
        _clinicService = clinicService;
        _specialityService = specialityService;
        _veterinarianService = veterinarianService;
        _appointmentService = appointmentService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? specialityId, int page = 1)
    {
        var allClinics = (await _clinicService.GetAllAsync()).OrderBy(c => c.Name).ToList();
        if (specialityId.HasValue)
            allClinics = allClinics.Where(c => c.SpecialityId == specialityId.Value).ToList();

        var clinics = PaginatedList<ClinicDto>.Create(allClinics, page, PageSize);
        var specialities = (await _specialityService.GetAllAsync()).OrderBy(s => s.Name).ToList();

        ViewBag.CurrentPage = clinics.PageIndex;
        ViewBag.TotalPages = clinics.TotalPages;
        ViewBag.RouteValues = new Dictionary<string, string?> { ["specialityId"] = specialityId?.ToString() };

        return View(new ClinicsIndexViewModel
        {
            Clinics = clinics,
            Specialities = specialities,
            SelectedSpecialityId = specialityId
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var clinic = await _clinicService.GetByIdAsync(id);
        if (clinic == null) return NotFound();

        var vets = (await _veterinarianService.GetAllAsync())
            .Where(v => v.ClinicId == id)
            .OrderBy(v => v.LastName)
            .ToList();

        var appointments = await _appointmentService.GetAllAsync(clinicId: id);

        return View(new ClinicDetailsViewModel
        {
            Clinic = clinic,
            Veterinarians = vets,
            AppointmentCount = appointments.Count()
        });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var vm = new ClinicFormViewModel
        {
            SpecialityOptions = new SelectList(
                (await _specialityService.GetAllAsync()).OrderBy(s => s.Name), "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClinicFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.SpecialityOptions = new SelectList(
                (await _specialityService.GetAllAsync()).OrderBy(s => s.Name), "Id", "Name", vm.SpecialityId);
            return View(vm);
        }

        await _clinicService.CreateAsync(new ClinicWriteDto
        {
            Name = vm.Name,
            Address = vm.Address,
            Phone = vm.Phone,
            Email = vm.Email,
            Website = vm.Website,
            Description = vm.Description,
            SpecialityId = vm.SpecialityId
        });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var clinic = await _clinicService.GetByIdAsync(id);
        if (clinic == null) return NotFound();

        return View(new ClinicFormViewModel
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Address = clinic.Address,
            Phone = clinic.Phone,
            Email = clinic.Email,
            Website = clinic.Website,
            Description = clinic.Description,
            SpecialityId = clinic.SpecialityId,
            SpecialityOptions = new SelectList(
                (await _specialityService.GetAllAsync()).OrderBy(s => s.Name), "Id", "Name", clinic.SpecialityId)
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClinicFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.SpecialityOptions = new SelectList(
                (await _specialityService.GetAllAsync()).OrderBy(s => s.Name), "Id", "Name", vm.SpecialityId);
            return View(vm);
        }

        await _clinicService.UpdateAsync(id, new ClinicWriteDto
        {
            Name = vm.Name,
            Address = vm.Address,
            Phone = vm.Phone,
            Email = vm.Email,
            Website = vm.Website,
            Description = vm.Description,
            SpecialityId = vm.SpecialityId
        });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var clinic = await _clinicService.GetByIdAsync(id);
        if (clinic == null) return NotFound();
        return View(clinic);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _clinicService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
