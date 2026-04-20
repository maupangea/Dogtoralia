using DogtoraliaMVC.Controllers.Api.Controllers;
using DogtoraliaMVC.Controllers.Api.Dtos;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers.Api;

public class ClinicsApiControllerTests
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

    [Fact]
    public async Task GetAll_ReturnsAllSeededClinics()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<ClinicDto>>(result!.Value);
        Assert.Equal(6, list.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsClinicWithSpecialityName()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<ClinicDto>(result!.Value);
        Assert.Equal(1, dto.Id);
        Assert.NotEmpty(dto.SpecialityName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var dto = new ClinicWriteDto
        {
            Name = "New Clinic",
            Address = "123 Main St",
            Phone = "555-0001",
            Email = "new@clinic.com",
            SpecialityId = 1
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<ClinicDto>(created.Value);
        Assert.Equal("New Clinic", returned.Name);
        Assert.Equal(7, ctx.Clinics.Count());
    }

    [Fact]
    public async Task Create_WithInvalidSpeciality_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var dto = new ClinicWriteDto
        {
            Name = "X",
            Address = "X",
            Phone = "X",
            Email = "x@x.com",
            SpecialityId = 999
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var dto = new ClinicWriteDto
        {
            Name = "Updated",
            Address = "456 Ave",
            Phone = "555-9999",
            Email = "up@clinic.com",
            SpecialityId = 1
        };

        var result = await controller.Update(1, dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Updated", ctx.Clinics.Find(1)!.Name);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.Update(999, new ClinicWriteDto
        {
            Name = "X", Address = "X", Phone = "X", Email = "x@x.com", SpecialityId = 1
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_Deletes()
    {
        using var ctx = CreateContext();
        var sp = ctx.Specialities.First();
        var clinic = new Clinic { Name = "Temp", Address = "A", Phone = "1", Email = "t@t.com", SpecialityId = sp.Id, CreatedAt = DateTime.UtcNow };
        ctx.Clinics.Add(clinic);
        await ctx.SaveChangesAsync();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.Delete(clinic.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.Clinics.Find(clinic.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsApiController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
