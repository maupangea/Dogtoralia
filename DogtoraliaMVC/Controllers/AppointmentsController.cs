using DogtoraliaMVC.Data;
using DogtoraliaMVC.Helpers;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers;

public class AppointmentsController : Controller
{
    private readonly DogtoraliaDbContext _context;
    private const int PageSize = 8;

    public AppointmentsController(DogtoraliaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? clinicId, AppointmentStatus? status, int page = 1)
    {
        var query = _context.Appointments
            .Include(a => a.Clinic)
            .Include(a => a.Pet)
            .Include(a => a.Veterinarian)
            .AsQueryable();

        if (clinicId.HasValue)
            query = query.Where(a => a.ClinicId == clinicId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        query = query.OrderByDescending(a => a.AppointmentDate);

        var appointments = await PaginatedList<Appointment>.CreateAsync(query, page, PageSize);
        var clinics = await _context.Clinics.OrderBy(c => c.Name).ToListAsync();
        var availableStatuses = Enum.GetValues<AppointmentStatus>().ToList();

        var vm = new AppointmentsIndexViewModel
        {
            Appointments = appointments,
            SelectedClinicId = clinicId,
            SelectedStatus = status,
            Clinics = clinics,
            AvailableStatuses = availableStatuses
        };

        ViewBag.CurrentPage = appointments.PageIndex;
        ViewBag.TotalPages = appointments.TotalPages;
        ViewBag.RouteValues = new Dictionary<string, string?>
        {
            ["clinicId"] = clinicId?.ToString(),
            ["status"] = status?.ToString()
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Clinic)
            .Include(a => a.Pet)
            .Include(a => a.Veterinarian)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return NotFound();
        return View(appointment);
    }

    public async Task<IActionResult> Create(int? clinicId)
    {
        var vm = new AppointmentFormViewModel
        {
            ClinicId = clinicId ?? 0,
            AppointmentDate = DateTime.Today.AddDays(1),
            Status = AppointmentStatus.Pending
        };

        await PopulateFormSelectLists(vm, clinicId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFormSelectLists(vm, vm.ClinicId);
            return View(vm);
        }

        var appointment = new Appointment
        {
            ClinicId = vm.ClinicId,
            PetId = vm.PetId,
            VeterinarianId = vm.VeterinarianId,
            AppointmentDate = vm.AppointmentDate,
            Reason = vm.Reason,
            Notes = vm.Notes,
            Status = vm.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Clinics", new { id = vm.ClinicId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

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

        await PopulateFormSelectLists(vm, appointment.ClinicId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppointmentFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateFormSelectLists(vm, vm.ClinicId);
            return View(vm);
        }

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.ClinicId = vm.ClinicId;
        appointment.PetId = vm.PetId;
        appointment.VeterinarianId = vm.VeterinarianId;
        appointment.AppointmentDate = vm.AppointmentDate;
        appointment.Reason = vm.Reason;
        appointment.Notes = vm.Notes;
        appointment.Status = vm.Status;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Appointments.AnyAsync(a => a.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Clinic)
            .Include(a => a.Pet)
            .Include(a => a.Veterinarian)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return NotFound();
        return View(appointment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateFormSelectLists(AppointmentFormViewModel vm, int? clinicId)
    {
        vm.ClinicOptions = new SelectList(
            await _context.Clinics.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", vm.ClinicId);

        vm.PetOptions = new SelectList(
            await _context.Pets.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", vm.PetId);

        var vets = clinicId.HasValue && clinicId.Value > 0
            ? await _context.Veterinarians.Where(v => v.ClinicId == clinicId.Value).OrderBy(v => v.LastName).ToListAsync()
            : await _context.Veterinarians.OrderBy(v => v.LastName).ToListAsync();

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
