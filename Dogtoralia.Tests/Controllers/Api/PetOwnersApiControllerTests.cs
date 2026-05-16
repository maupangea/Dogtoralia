using Dogtoralia.Api.Controllers;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Tests.Controllers.Api;

public class PetOwnersControllerTests
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
    public async Task GetAll_ReturnsAllSeededOwners()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<PetOwnerDto>>(result!.Value);
        Assert.Equal(10, list.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOwner()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<PetOwnerDto>(result!.Value);
        Assert.Equal(1, dto.Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Create(new PetOwnerWriteDto
        {
            Name = "New Owner",
            Email = "new.owner@test.com",
            Phone = "555-9999"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<PetOwnerDto>(created.Value);
        Assert.Equal("New Owner", dto.Name);
        Assert.Equal(11, ctx.PetOwners.Count());
    }

    [Fact]
    public async Task Create_WithDuplicateEmail_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var existingEmail = ctx.PetOwners.First().Email;

        var result = await controller.Create(new PetOwnerWriteDto
        {
            Name = "Dup",
            Email = existingEmail,
            Phone = "555-0000"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Update(1, new PetOwnerWriteDto
        {
            Name = "Updated Name",
            Email = "updated@test.com",
            Phone = "555-1111"
        });

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Updated Name", ctx.PetOwners.Find(1)!.Name);
    }

    [Fact]
    public async Task Update_WithEmailUsedByAnotherOwner_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var otherEmail = ctx.PetOwners.Skip(1).First().Email;

        var result = await controller.Update(1, new PetOwnerWriteDto
        {
            Name = "X",
            Email = otherEmail,
            Phone = "555-0000"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Update(999, new PetOwnerWriteDto
        {
            Name = "X", Email = "x@x.com", Phone = "X"
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExistsWithNoPets_Deletes()
    {
        using var ctx = CreateContext();
        var owner = new PetOwner { Name = "Temp", Email = "temp@del.com", Phone = "555-0000", CreatedAt = DateTime.UtcNow };
        ctx.PetOwners.Add(owner);
        await ctx.SaveChangesAsync();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Delete(owner.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.PetOwners.Find(owner.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
