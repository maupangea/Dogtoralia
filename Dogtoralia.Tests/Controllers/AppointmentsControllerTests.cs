using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Shared.Models;
using Dogtoralia.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class AppointmentsControllerTests
{
    private static IEnumerable<AppointmentDto> SeedAppointments(int count = 5) =>
        Enumerable.Range(1, count).Select(i => new AppointmentDto
        {
            Id = i, ClinicId = 1, ClinicName = "Clinic", PetId = i, PetName = $"Pet {i}",
            PetOwnerId = i, PetSpecies = "Perro", PetBreed = "Mix",
            AppointmentDate = DateTime.Today.AddDays(i),
            Reason = "Revisión", Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

    private static IEnumerable<ClinicDto> SeedClinics() => new List<ClinicDto>
    {
        new() { Id = 1, Name = "Clinic 1", Address = "A", Phone = "1", Email = "c@x.com", SpecialityId = 1, CreatedAt = DateTime.UtcNow }
    };

    private static AppointmentsController CreateController(
        Mock<IAppointmentService>? apptSvc = null,
        Mock<IClinicService>? clinicSvc = null,
        Mock<IVeterinarianService>? vetSvc = null,
        Mock<IPetService>? petSvc = null,
        Mock<IPetOwnerService>? ownerSvc = null,
        Mock<UserManager<IdentityUser>>? mockUm = null)
    {
        apptSvc ??= new Mock<IAppointmentService>();
        clinicSvc ??= new Mock<IClinicService>();
        vetSvc ??= new Mock<IVeterinarianService>();
        petSvc ??= new Mock<IPetService>();
        ownerSvc ??= new Mock<IPetOwnerService>();
        mockUm ??= ControllerTestHelpers.CreateMockUserManager();

        apptSvc.Setup(s => s.GetAllAsync(It.IsAny<int?>(), It.IsAny<AppointmentStatus?>()))
               .ReturnsAsync(SeedAppointments());
        clinicSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedClinics());
        vetSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<VeterinarianDto>());
        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetDto>());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetOwnerDto>());

        var controller = new AppointmentsController(
            apptSvc.Object, clinicSvc.Object, vetSvc.Object,
            petSvc.Object, ownerSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetAdminUser(controller);
        return controller;
    }

    [Fact]
    public async Task Index_NoFilter_ReturnsAllAppointments()
    {
        var controller = CreateController();
        var result = await controller.Index(null, null, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.NotNull(vm);
        Assert.True(vm!.Appointments.Count > 0);
    }

    [Fact]
    public async Task Index_FilterByClinic_PassesFilterToService()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetAllAsync(1, null)).ReturnsAsync(SeedAppointments(2));
        var controller = CreateController(apptSvc);
        var result = await controller.Index(1, null, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.Equal(1, vm!.SelectedClinicId);
    }

    [Fact]
    public async Task Details_ValidId_ReturnsAppointmentDto()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedAppointments(1).First());
        var controller = CreateController(apptSvc);
        var result = await controller.Details(1) as ViewResult;

        Assert.IsType<AppointmentDto>(result!.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((AppointmentDto?)null);
        var controller = CreateController(apptSvc);

        Assert.IsType<NotFoundResult>(await controller.Details(999));
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithSelectLists()
    {
        var controller = CreateController();
        var result = await controller.Create((int?)null) as ViewResult;
        var vm = result!.Model as AppointmentFormViewModel;

        Assert.NotNull(vm!.ClinicOptions);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToClinicDetails()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.CreateAsync(It.IsAny<AppointmentWriteDto>()))
               .ReturnsAsync(new AppointmentDto { Id = 10, ClinicId = 1 });
        var controller = CreateController(apptSvc);
        var vm = new AppointmentFormViewModel
        {
            ClinicId = 1, PetId = 1, AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Revisión", Status = AppointmentStatus.Pending
        };

        var result = await controller.Create(vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal("Clinics", result.ControllerName);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedAppointments(1).First());
        var controller = CreateController(apptSvc);
        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as AppointmentFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((AppointmentDto?)null);
        var controller = CreateController(apptSvc);

        Assert.IsType<NotFoundResult>(await controller.Edit(999));
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedAppointments(1).First());
        apptSvc.Setup(s => s.UpdateAsync(1, It.IsAny<AppointmentWriteDto>())).ReturnsAsync(true);
        var controller = CreateController(apptSvc);
        var vm = new AppointmentFormViewModel
        {
            Id = 1, ClinicId = 1, PetId = 1, AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Updated", Status = AppointmentStatus.Confirmed
        };

        Assert.IsType<RedirectToActionResult>(await controller.Edit(1, vm));
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        var controller = CreateController();

        Assert.IsType<BadRequestResult>(await controller.Edit(1, new AppointmentFormViewModel { Id = 2 }));
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsAppointmentDto()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedAppointments(1).First());
        var controller = CreateController(apptSvc);
        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<AppointmentDto>(result!.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidId_DeletesAndRedirects()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var controller = CreateController(apptSvc);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        apptSvc.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
