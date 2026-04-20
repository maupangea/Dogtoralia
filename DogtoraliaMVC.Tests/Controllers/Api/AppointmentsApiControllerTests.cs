using DogtoraliaMVC.Controllers.Api.Controllers;
using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers.Api;

public class AppointmentsApiControllerTests
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

    private static async Task<Appointment> AddAppointmentAsync(
        DogtoraliaDbContext ctx,
        int clinicId = 1, int petId = 1, int? vetId = null,
        AppointmentStatus status = AppointmentStatus.Pending)
    {
        var appt = new Appointment
        {
            ClinicId = clinicId,
            PetId = petId,
            VeterinarianId = vetId,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Checkup",
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Appointments.Add(appt);
        await ctx.SaveChangesAsync();
        return appt;
    }

    [Fact]
    public async Task GetAll_WithNoFilter_ReturnsAllAppointments()
    {
        using var ctx = CreateContext();
        await AddAppointmentAsync(ctx, clinicId: 1, petId: 1);
        await AddAppointmentAsync(ctx, clinicId: 2, petId: 2);
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.GetAll(null, null) as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(result!.Value);
        Assert.Equal(2, list.Count());
    }

    [Fact]
    public async Task GetAll_FilterByClinic_ReturnsOnlyMatchingClinic()
    {
        using var ctx = CreateContext();
        await AddAppointmentAsync(ctx, clinicId: 1, petId: 1);
        await AddAppointmentAsync(ctx, clinicId: 2, petId: 2);
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.GetAll(clinicId: 1, status: null) as OkObjectResult;

        var list = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(result!.Value);
        Assert.All(list, a => Assert.Equal(1, a.ClinicId));
    }

    [Fact]
    public async Task GetAll_FilterByStatus_ReturnsOnlyMatchingStatus()
    {
        using var ctx = CreateContext();
        await AddAppointmentAsync(ctx, status: AppointmentStatus.Pending);
        await AddAppointmentAsync(ctx, clinicId: 2, petId: 2, status: AppointmentStatus.Confirmed);
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.GetAll(null, AppointmentStatus.Confirmed) as OkObjectResult;

        var list = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(result!.Value);
        Assert.All(list, a => Assert.Equal(AppointmentStatus.Confirmed, a.Status));
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsAppointmentWithNames()
    {
        using var ctx = CreateContext();
        var appt = await AddAppointmentAsync(ctx);
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.GetById(appt.Id) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<AppointmentDto>(result!.Value);
        Assert.Equal(appt.Id, dto.Id);
        Assert.NotEmpty(dto.ClinicName);
        Assert.NotEmpty(dto.PetName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var dto = new AppointmentWriteDto
        {
            ClinicId = 1,
            PetId = 1,
            AppointmentDate = DateTime.Today.AddDays(3),
            Reason = "Annual checkup",
            Status = AppointmentStatus.Pending
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<AppointmentDto>(created.Value);
        Assert.Equal("Annual checkup", returned.Reason);
        Assert.Equal(1, ctx.Appointments.Count());
    }

    [Fact]
    public async Task Create_WithInvalidClinic_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.Create(new AppointmentWriteDto
        {
            ClinicId = 999, PetId = 1,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "X"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_WithInvalidPet_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.Create(new AppointmentWriteDto
        {
            ClinicId = 1, PetId = 999,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "X"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var appt = await AddAppointmentAsync(ctx);
        var controller = new AppointmentsApiController(ctx);

        var dto = new AppointmentWriteDto
        {
            ClinicId = 1, PetId = 1,
            AppointmentDate = DateTime.Today.AddDays(5),
            Reason = "Updated reason",
            Status = AppointmentStatus.Confirmed
        };

        var result = await controller.Update(appt.Id, dto);

        Assert.IsType<NoContentResult>(result);
        var updated = ctx.Appointments.Find(appt.Id)!;
        Assert.Equal("Updated reason", updated.Reason);
        Assert.Equal(AppointmentStatus.Confirmed, updated.Status);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.Update(999, new AppointmentWriteDto
        {
            ClinicId = 1, PetId = 1,
            AppointmentDate = DateTime.Today,
            Reason = "X"
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_Deletes()
    {
        using var ctx = CreateContext();
        var appt = await AddAppointmentAsync(ctx);
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.Delete(appt.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.Appointments.Find(appt.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new AppointmentsApiController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
