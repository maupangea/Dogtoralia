using Dogtoralia.Api.Controllers;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Tests.Controllers.Api;

public class VeterinariansControllerTests
{
    private static DogtoraliaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DogtoraliaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DogtoraliaContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static VeterinarianWriteDto ValidWriteDto(int clinicId = 1) => new()
    {
        FirstName = "Jane",
        LastName = "Smith",
        LicenseNumber = "LIC-UNIQUE-99",
        Email = "jane.smith@vet.com",
        Phone = "555-0099",
        YearsOfExperience = 5,
        ClinicId = clinicId
    };

    [Fact]
    public async Task GetAll_ReturnsAllSeededVeterinarians()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<VeterinarianDto>>(result!.Value);
        Assert.Equal(10, list.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsVetWithClinicName()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<VeterinarianDto>(result!.Value);
        Assert.Equal(1, dto.Id);
        Assert.NotEmpty(dto.ClinicName);
        Assert.NotEmpty(dto.FullName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.Create(ValidWriteDto());

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<VeterinarianDto>(created.Value);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal(11, ctx.Veterinarians.Count());
    }

    [Fact]
    public async Task Create_WithDuplicateLicense_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);
        var existingLicense = ctx.Veterinarians.First().LicenseNumber;

        var dto = ValidWriteDto();
        dto.LicenseNumber = existingLicense;

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_WithInvalidClinic_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.Create(ValidWriteDto(clinicId: 999));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var dto = ValidWriteDto();
        dto.FirstName = "UpdatedFirst";

        var result = await controller.Update(1, dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("UpdatedFirst", ctx.Veterinarians.Find(1)!.FirstName);
    }

    [Fact]
    public async Task Update_WithDuplicateLicenseOnOtherVet_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);
        var otherLicense = ctx.Veterinarians.Skip(1).First().LicenseNumber;

        var dto = ValidWriteDto();
        dto.LicenseNumber = otherLicense;

        var result = await controller.Update(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.Update(999, ValidWriteDto());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_Deletes()
    {
        using var ctx = CreateContext();
        var vet = new Veterinarian
        {
            FirstName = "Temp", LastName = "Vet", LicenseNumber = "TEMP-001",
            Email = "temp@vet.com", Phone = "555-0000", YearsOfExperience = 1, ClinicId = 1
        };
        ctx.Veterinarians.Add(vet);
        await ctx.SaveChangesAsync();
        var controller = new VeterinariansController(ctx);

        var result = await controller.Delete(vet.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.Veterinarians.Find(vet.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new VeterinariansController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
