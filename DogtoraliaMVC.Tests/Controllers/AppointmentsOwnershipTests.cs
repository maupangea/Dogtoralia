using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class AppointmentsOwnershipTests
{
    private const string UserOwnerId = "user-appt-id";

    private static DogtoraliaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DogtoraliaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DogtoraliaDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static async Task<PetOwner> AddOwnerWithUser(DogtoraliaDbContext ctx, string userId)
    {
        var owner = new PetOwner
        {
            Name = "Appt User",
            Email = "apptuser@example.com",
            Phone = "+52-55-0000-2222",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        ctx.PetOwners.Add(owner);
        await ctx.SaveChangesAsync();
        return owner;
    }

    private static async Task<Pet> AddPet(DogtoraliaDbContext ctx, int ownerId)
    {
        var pet = new Pet
        {
            Name = "UserPet",
            Species = "Gato",
            Breed = "Siamese",
            DateOfBirth = new DateTime(2022, 1, 1),
            PetOwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Pets.Add(pet);
        await ctx.SaveChangesAsync();
        return pet;
    }

    private static async Task<Appointment> AddAppointment(DogtoraliaDbContext ctx, int petId, int clinicId = 1)
    {
        var appt = new Appointment
        {
            ClinicId = clinicId,
            PetId = petId,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Revisión",
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Appointments.Add(appt);
        await ctx.SaveChangesAsync();
        return appt;
    }

    [Fact]
    public async Task Index_UserRole_ReturnsOnlyOwnAppointments()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var pet = await AddPet(ctx, owner.Id);
        var appt = await AddAppointment(ctx, pet.Id);
        // Appointments for seeded pets (PetOwnerId 1–10) should not appear

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new AppointmentsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Index(null, null, 1) as ViewResult;
        var vm = result!.Model as DogtoraliaMVC.ViewModels.AppointmentsIndexViewModel;

        Assert.Equal(1, vm!.Appointments.Count);
        Assert.Equal(appt.Id, vm.Appointments[0].Id);
    }

    [Fact]
    public async Task Details_UserRole_OwnAppointment_ReturnsView()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var pet = await AddPet(ctx, owner.Id);
        var appt = await AddAppointment(ctx, pet.Id);

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new AppointmentsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Details(appt.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OtherAppointment_ReturnsForbid()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        // Create an appointment for seeded pet Id=1 (owned by seeded owner, not our user)
        var otherAppt = await AddAppointment(ctx, petId: 1);

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new AppointmentsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Details(otherAppt.Id);

        Assert.IsType<ForbidResult>(result);
    }
}
