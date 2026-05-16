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

public class AppointmentsOwnershipTests
{
    private const string UserOwnerId = "user-appt-id";
    private const int UserLinkedOwnerId = 99;

    private static PetOwnerDto UserOwner => new()
    {
        Id = UserLinkedOwnerId, Name = "Appt User", Email = "apptuser@example.com",
        Phone = "1", UserId = UserOwnerId, PetCount = 1, CreatedAt = DateTime.UtcNow
    };

    private static AppointmentDto UserAppointment => new()
    {
        Id = 50, ClinicId = 1, ClinicName = "Clinic", PetId = 50, PetName = "UserPet",
        PetOwnerId = UserLinkedOwnerId, PetSpecies = "Gato", PetBreed = "Siamese",
        AppointmentDate = DateTime.Today.AddDays(1), Reason = "Revisión",
        Status = AppointmentStatus.Pending, CreatedAt = DateTime.UtcNow
    };

    private static AppointmentDto OtherAppointment => new()
    {
        Id = 1, ClinicId = 1, ClinicName = "Clinic", PetId = 1, PetName = "OtherPet",
        PetOwnerId = 1, PetSpecies = "Perro", PetBreed = "Lab",
        AppointmentDate = DateTime.Today.AddDays(1), Reason = "Check",
        Status = AppointmentStatus.Pending, CreatedAt = DateTime.UtcNow
    };

    private static AppointmentsController CreateUserController(
        Mock<IAppointmentService> apptSvc,
        Mock<IClinicService>? clinicSvc = null,
        Mock<IVeterinarianService>? vetSvc = null,
        Mock<IPetService>? petSvc = null,
        Mock<IPetOwnerService>? ownerSvc = null)
    {
        clinicSvc ??= new Mock<IClinicService>();
        vetSvc ??= new Mock<IVeterinarianService>();
        petSvc ??= new Mock<IPetService>();
        ownerSvc ??= new Mock<IPetOwnerService>();

        clinicSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ClinicDto>());
        vetSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<VeterinarianDto>());
        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetDto>());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetOwnerDto> { UserOwner });

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new AppointmentsController(
            apptSvc.Object, clinicSvc.Object, vetSvc.Object,
            petSvc.Object, ownerSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);
        return controller;
    }

    [Fact]
    public async Task Index_UserRole_ReturnsOnlyOwnAppointments()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetAllAsync(null, null))
               .ReturnsAsync(new List<AppointmentDto> { UserAppointment, OtherAppointment });
        var controller = CreateUserController(apptSvc);

        var result = await controller.Index(null, null, 1) as ViewResult;
        var vm = result!.Model as AppointmentsIndexViewModel;

        Assert.Equal(1, vm!.Appointments.Count);
        Assert.Equal(UserAppointment.Id, vm.Appointments[0].Id);
    }

    [Fact]
    public async Task Details_UserRole_OwnAppointment_ReturnsView()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(UserAppointment.Id)).ReturnsAsync(UserAppointment);
        var controller = CreateUserController(apptSvc);

        Assert.IsType<ViewResult>(await controller.Details(UserAppointment.Id));
    }

    [Fact]
    public async Task Details_UserRole_OtherAppointment_ReturnsForbid()
    {
        var apptSvc = new Mock<IAppointmentService>();
        apptSvc.Setup(s => s.GetByIdAsync(OtherAppointment.Id)).ReturnsAsync(OtherAppointment);
        var controller = CreateUserController(apptSvc);

        Assert.IsType<ForbidResult>(await controller.Details(OtherAppointment.Id));
    }
}
