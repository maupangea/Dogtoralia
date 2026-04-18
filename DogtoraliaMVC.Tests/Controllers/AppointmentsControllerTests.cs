using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.Tests.Helpers;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class AppointmentsControllerTests
{
    private static DogtoraliaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DogtoraliaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DogtoraliaDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static AppointmentsController CreateController(DogtoraliaDbContext ctx)
    {
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new AppointmentsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetAdminUser(controller);
        return controller;
    }

    private static async Task<Appointment> AddAppointmentAsync(DogtoraliaDbContext ctx,
        int clinicId = 1, int petId = 1, int? vetId = 1,
        AppointmentStatus status = AppointmentStatus.Pending,
        DateTime? date = null)
    {
        var appointment = new Appointment
        {
            ClinicId = clinicId,
            PetId = petId,
            VeterinarianId = vetId,
            AppointmentDate = date ?? DateTime.Today.AddDays(1),
            Reason = "Revisión general",
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Appointments.Add(appointment);
        await ctx.SaveChangesAsync();
        return appointment;
    }

    // ── Index ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Index_NoFilter_ReturnsAllAppointments()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);

        var result = await controller.Index(null, null, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.NotNull(vm);
        Assert.Equal(0, vm!.Appointments.Count);
    }

    [Fact]
    public async Task Index_FilterByClinicId_ReturnsOnlyThatClinic()
    {
        using var ctx = CreateContext();
        await AddAppointmentAsync(ctx, clinicId: 1);
        await AddAppointmentAsync(ctx, clinicId: 2, petId: 2, vetId: 3);
        var controller = CreateController(ctx);

        var result = await controller.Index(1, null, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.All(vm!.Appointments, a => Assert.Equal(1, a.ClinicId));
        Assert.Equal(1, vm.Appointments.Count);
    }

    [Fact]
    public async Task Index_FilterByStatus_ReturnsOnlyThatStatus()
    {
        using var ctx = CreateContext();
        await AddAppointmentAsync(ctx, status: AppointmentStatus.Pending);
        await AddAppointmentAsync(ctx, status: AppointmentStatus.Confirmed);
        var controller = CreateController(ctx);

        var result = await controller.Index(null, AppointmentStatus.Confirmed, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.All(vm!.Appointments, a => Assert.Equal(AppointmentStatus.Confirmed, a.Status));
        Assert.Equal(1, vm.Appointments.Count);
    }

    [Fact]
    public async Task Index_Pagination_ReturnsCorrectPage()
    {
        using var ctx = CreateContext();
        // Add 9 appointments (page size = 8, page 2 should have 1)
        for (int i = 0; i < 9; i++)
            await AddAppointmentAsync(ctx, date: DateTime.Today.AddDays(i + 1));

        var controller = CreateController(ctx);

        var result = await controller.Index(null, null, 2) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.Equal(1, vm!.Appointments.Count);
        Assert.Equal(2, vm.Appointments.PageIndex);
        Assert.Equal(2, vm.Appointments.TotalPages);
    }

    // ── Details ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Details_ValidId_ReturnsAppointment()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);

        var result = await controller.Details(appointment.Id) as ViewResult;

        Assert.NotNull(result);
        var model = result!.Model as Appointment;
        Assert.Equal(appointment.Id, model!.Id);
        Assert.Equal("Revisión general", model.Reason);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Create GET ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Get_NoClinicId_ReturnsViewWithSelectLists()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);

        var result = await controller.Create((int?)null) as ViewResult;
        var vm = result!.Model as AppointmentFormViewModel;

        Assert.NotNull(vm);
        Assert.NotNull(vm!.ClinicOptions);
        Assert.NotNull(vm.PetOptions);
        Assert.NotNull(vm.VeterinarianOptions);
        Assert.NotNull(vm.StatusOptions);
    }

    [Fact]
    public async Task Create_Get_WithClinicId_PreSelectsClinicAndFiltersVets()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);

        var result = await controller.Create(1) as ViewResult;
        var vm = result!.Model as AppointmentFormViewModel;

        Assert.Equal(1, vm!.ClinicId);
        // VeterinarianOptions should only include vets from clinic 1 (2 vets in seed)
        var vetCount = vm.VeterinarianOptions!.Count();
        Assert.Equal(2, vetCount);
    }

    // ── Create POST ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Post_ValidModel_AddsAndRedirectsToClinicDetails()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);
        var vm = new AppointmentFormViewModel
        {
            ClinicId = 1,
            PetId = 1,
            VeterinarianId = 1,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Vacunación anual",
            Status = AppointmentStatus.Pending
        };

        var result = await controller.Create(vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal("Clinics", result.ControllerName);
        Assert.Equal(1, result.RouteValues!["id"]);
        Assert.Equal(1, ctx.Appointments.Count());
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);
        controller.ModelState.AddModelError("Reason", "Required");
        var vm = new AppointmentFormViewModel { ClinicId = 1, PetId = 1 };

        var result = await controller.Create(vm);

        Assert.IsType<ViewResult>(result);
    }

    // ── Edit GET ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsPopulatedViewModel()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);

        var result = await controller.Edit(appointment.Id) as ViewResult;
        var vm = result!.Model as AppointmentFormViewModel;

        Assert.Equal(appointment.Id, vm!.Id);
        Assert.Equal(appointment.ClinicId, vm.ClinicId);
        Assert.Equal(appointment.PetId, vm.PetId);
        Assert.Equal(appointment.Reason, vm.Reason);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = CreateController(ctx);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Edit POST ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirectsToIndex()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);
        var vm = new AppointmentFormViewModel
        {
            Id = appointment.Id,
            ClinicId = appointment.ClinicId,
            PetId = appointment.PetId,
            VeterinarianId = appointment.VeterinarianId,
            AppointmentDate = appointment.AppointmentDate,
            Reason = "Motivo actualizado",
            Status = AppointmentStatus.Confirmed
        };

        var result = await controller.Edit(appointment.Id, vm) as RedirectToActionResult;

        Assert.Equal("Index", result!.ActionName);
        var updated = await ctx.Appointments.FindAsync(appointment.Id);
        Assert.Equal("Motivo actualizado", updated!.Reason);
        Assert.Equal(AppointmentStatus.Confirmed, updated.Status);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);
        var vm = new AppointmentFormViewModel { Id = 999 };

        var result = await controller.Edit(appointment.Id, vm);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);
        controller.ModelState.AddModelError("Reason", "Required");
        var vm = new AppointmentFormViewModel { Id = appointment.Id, ClinicId = 1 };

        var result = await controller.Edit(appointment.Id, vm);

        Assert.IsType<ViewResult>(result);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsAppointment()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);

        var result = await controller.Delete(appointment.Id) as ViewResult;
        var model = result!.Model as Appointment;

        Assert.Equal(appointment.Id, model!.Id);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidId_RemovesAndRedirects()
    {
        using var ctx = CreateContext();
        var appointment = await AddAppointmentAsync(ctx);
        var controller = CreateController(ctx);

        var result = await controller.DeleteConfirmed(appointment.Id) as RedirectToActionResult;

        Assert.Equal("Index", result!.ActionName);
        Assert.Equal(0, ctx.Appointments.Count());
    }
}
