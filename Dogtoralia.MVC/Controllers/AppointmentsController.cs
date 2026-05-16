using Dogtoralia.MVC.Helpers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dogtoralia.MVC.Controllers;

public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IClinicService _clinicService;
    private readonly IVeterinarianService _veterinarianService;
    private readonly IPetService _petService;
    private readonly IPetOwnerService _petOwnerService;
    private readonly UserManager<IdentityUser> _userManager;
    private const int PageSize = 8;

    public AppointmentsController(
        IAppointmentService appointmentService,
        IClinicService clinicService,
        IVeterinarianService veterinarianService,
        IPetService petService,
        IPetOwnerService petOwnerService,
        UserManager<IdentityUser> userManager)
    {
        _appointmentService = appointmentService;
        _clinicService = clinicService;
        _veterinarianService = veterinarianService;
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

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index(int? clinicId, AppointmentStatus? status, int page = 1)
    {
        var allAppointments = (await _appointmentService.GetAllAsync(clinicId, status)).ToList();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner == null) return Forbid();
            allAppointments = allAppointments.Where(a => a.PetOwnerId == currentOwner.Id).ToList();
        }

        var sorted = allAppointments.OrderByDescending(a => a.AppointmentDate).ToList();
        var appointments = PaginatedList<AppointmentDto>.Create(sorted, page, PageSize);
        var clinics = (await _clinicService.GetAllAsync()).OrderBy(c => c.Name).ToList();
        var availableStatuses = Enum.GetValues<AppointmentStatus>().ToList();

        ViewBag.CurrentPage = appointments.PageIndex;
        ViewBag.TotalPages = appointments.TotalPages;
        ViewBag.RouteValues = new Dictionary<string, string?>
        {
            ["clinicId"] = clinicId?.ToString(),
            ["status"] = status?.ToString()
        };

        return View(new AppointmentsIndexViewModel
        {
            Appointments = appointments,
            SelectedClinicId = clinicId,
            SelectedStatus = status,
            Clinics = clinics,
            AvailableStatuses = availableStatuses
        });
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Details(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (appointment.PetOwnerId != currentOwner?.Id) return Forbid();
        }

        return View(appointment);
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Create(int? clinicId)
    {
        var vm = new AppointmentFormViewModel
        {
            ClinicId = clinicId ?? 0,
            AppointmentDate = DateTime.Today.AddDays(1),
            Status = AppointmentStatus.Pending
        };

        int? restrictToPetOwnerId = null;
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner == null) return Forbid();
            restrictToPetOwnerId = currentOwner.Id;
        }

        await PopulateFormSelectLists(vm, clinicId, restrictToPetOwnerId);
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentFormViewModel vm)
    {
        int? restrictToPetOwnerId = null;
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (currentOwner == null) return Forbid();
            restrictToPetOwnerId = currentOwner.Id;

            var pet = await _petService.GetByIdAsync(vm.PetId);
            if (pet?.PetOwnerId != currentOwner.Id) return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await PopulateFormSelectLists(vm, vm.ClinicId, restrictToPetOwnerId);
            return View(vm);
        }

        await _appointmentService.CreateAsync(new AppointmentWriteDto
        {
            ClinicId = vm.ClinicId,
            PetId = vm.PetId,
            VeterinarianId = vm.VeterinarianId,
            AppointmentDate = vm.AppointmentDate,
            Reason = vm.Reason,
            Notes = vm.Notes,
            Status = vm.Status
        });

        return RedirectToAction("Details", "Clinics", new { id = vm.ClinicId });
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Edit(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        int? restrictToPetOwnerId = null;
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (appointment.PetOwnerId != currentOwner?.Id) return Forbid();
            restrictToPetOwnerId = currentOwner.Id;
        }

        var vm = new AppointmentFormViewModel
        {
            Id = appointment.Id,
            ClinicId = appointment.ClinicId,
            PetId = appointment.PetId,
            VeterinarianId = appointment.VeterinarianId,
            AppointmentDate = appointment.AppointmentDate,
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            Status = appointment.Status
        };

        await PopulateFormSelectLists(vm, appointment.ClinicId, restrictToPetOwnerId);
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppointmentFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        int? restrictToPetOwnerId = null;
        if (User.IsInRole("User"))
        {
            var currentOwner = await GetCurrentUserOwnerAsync();
            if (appointment.PetOwnerId != currentOwner?.Id) return Forbid();
            restrictToPetOwnerId = currentOwner.Id;
        }

        if (!ModelState.IsValid)
        {
            await PopulateFormSelectLists(vm, vm.ClinicId, restrictToPetOwnerId);
            return View(vm);
        }

        await _appointmentService.UpdateAsync(id, new AppointmentWriteDto
        {
            ClinicId = vm.ClinicId,
            PetId = vm.PetId,
            VeterinarianId = vm.VeterinarianId,
            AppointmentDate = vm.AppointmentDate,
            Reason = vm.Reason,
            Notes = vm.Notes,
            Status = vm.Status
        });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();
        return View(appointment);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _appointmentService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateFormSelectLists(AppointmentFormViewModel vm, int? clinicId, int? petOwnerId = null)
    {
        var clinics = (await _clinicService.GetAllAsync()).OrderBy(c => c.Name).ToList();
        vm.ClinicOptions = new SelectList(clinics, "Id", "Name", vm.ClinicId);

        var allPets = (await _petService.GetAllAsync()).OrderBy(p => p.Name).ToList();
        if (petOwnerId.HasValue)
            allPets = allPets.Where(p => p.PetOwnerId == petOwnerId.Value).ToList();
        vm.PetOptions = new SelectList(allPets, "Id", "Name", vm.PetId);

        var allVets = await _veterinarianService.GetAllAsync();
        var vets = (clinicId.HasValue && clinicId.Value > 0
            ? allVets.Where(v => v.ClinicId == clinicId.Value)
            : allVets).OrderBy(v => v.LastName).ToList();
        vm.VeterinarianOptions = new SelectList(vets, "Id", "FullName", vm.VeterinarianId);

        var statusItems = Enum.GetValues<AppointmentStatus>()
            .Select(s => new { Value = (int)s, Text = GetStatusDisplayName(s) })
            .ToList();
        vm.StatusOptions = new SelectList(statusItems, "Value", "Text", (int)vm.Status);
    }

    private static string GetStatusDisplayName(AppointmentStatus s) => s switch
    {
        AppointmentStatus.Pending => "Pendiente",
        AppointmentStatus.Confirmed => "Confirmada",
        AppointmentStatus.Cancelled => "Cancelada",
        AppointmentStatus.Completed => "Completada",
        _ => s.ToString()
    };
}
