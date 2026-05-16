using Dogtoralia.Api.Controllers;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Api.Data;
using Dogtoralia.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogtoralia.Tests.Controllers.Api;

public class PetsControllerTests
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

    private static PetWriteDto ValidWriteDto(int ownerId = 1) => new()
    {
        Name = "Buddy",
        Species = "Perro",
        Breed = "Labrador",
        DateOfBirth = new DateTime(2020, 1, 1),
        PetOwnerId = ownerId
    };

    [Fact]
    public async Task GetAll_ReturnsAllSeededPets()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        var list = Assert.IsAssignableFrom<IEnumerable<PetDto>>(result!.Value);
        Assert.Equal(10, list.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsPetWithOwnerName()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        var dto = Assert.IsType<PetDto>(result!.Value);
        Assert.Equal(1, dto.Id);
        Assert.NotEmpty(dto.PetOwnerName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201AndPersists()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Create(ValidWriteDto());

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<PetDto>(created.Value);
        Assert.Equal("Buddy", dto.Name);
        Assert.Equal(11, ctx.Pets.Count());
    }

    [Fact]
    public async Task Create_WithInvalidOwner_Returns400()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Create(ValidWriteDto(ownerId: 999));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenExists_Updates()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var dto = ValidWriteDto();
        dto.Name = "UpdatedPet";

        var result = await controller.Update(1, dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("UpdatedPet", ctx.Pets.Find(1)!.Name);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Update(999, ValidWriteDto());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_Deletes()
    {
        using var ctx = CreateContext();
        var pet = new Pet
        {
            Name = "Temp", Species = "Gato", Breed = "Mix",
            DateOfBirth = new DateTime(2021, 6, 1),
            PetOwnerId = 1, CreatedAt = DateTime.UtcNow
        };
        ctx.Pets.Add(pet);
        await ctx.SaveChangesAsync();
        var controller = new PetsController(ctx);

        var result = await controller.Delete(pet.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(ctx.Pets.Find(pet.Id));
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
