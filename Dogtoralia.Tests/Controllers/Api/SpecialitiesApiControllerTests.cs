using Dogtoralia.Api.Controllers;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Tests.Controllers.Api;

public class SpecialitiesControllerTests
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

    [Fact]
    public async Task GetAll_ReturnsAllSeededSpecialities()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<SpecialityDto>>(result!.Value);
        Assert.Equal(5, list.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsSpeciality()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<SpecialityDto>(result!.Value);
        Assert.Equal(1, dto.Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.Create(new SpecialityWriteDto { Name = "Oncology" });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<SpecialityDto>(created.Value);
        Assert.Equal("Oncology", dto.Name);
        Assert.Equal(6, ctx.Specialities.Count());
    }

    [Fact]
    public async Task Create_WithInvalidModel_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new SpecialityWriteDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.Update(1, new SpecialityWriteDto { Name = "Updated" });

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Updated", ctx.Specialities.Find(1)!.Name);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.Update(999, new SpecialityWriteDto { Name = "X" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_Deletes()
    {
        using var ctx = CreateContext();
        var sp = new Speciality { Name = "Temp" };
        ctx.Specialities.Add(sp);
        await ctx.SaveChangesAsync();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.Delete(sp.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.Specialities.Find(sp.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new SpecialitiesController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
