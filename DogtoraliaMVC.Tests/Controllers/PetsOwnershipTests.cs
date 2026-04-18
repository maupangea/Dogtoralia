using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.Tests.Helpers;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class PetsOwnershipTests
{
    private const string UserOwnerId = "user-pets-id";

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
            Name = "User Owner",
            Email = "userowner@example.com",
            Phone = "+52-55-0000-1111",
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
            Name = "MyPet",
            Species = "Perro",
            Breed = "Labrador",
            DateOfBirth = new DateTime(2022, 1, 1),
            PetOwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Pets.Add(pet);
        await ctx.SaveChangesAsync();
        return pet;
    }

    [Fact]
    public async Task Index_UserRole_ReturnsOnlyOwnPets()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        await AddPet(ctx, owner.Id);
        // Seeded pets belong to seeded owners (PetOwnerId 1–10), not to our user-linked owner

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.All(vm!.Pets, p => Assert.Equal(owner.Id, p.PetOwnerId));
        Assert.Equal(1, vm.Pets.Count);
    }

    [Fact]
    public async Task Create_Get_UserRole_OtherOwner_ReturnsForbid()
    {
        using var ctx = CreateContext();
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        // Try to create a pet for seeded owner Id=1 (not theirs)
        var result = await controller.Create(petOwnerId: 1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OwnPet_ReturnsView()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var pet = await AddPet(ctx, owner.Id);

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Details(pet.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OtherPet_ReturnsForbid()
    {
        using var ctx = CreateContext();
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        // Seeded pet Id=1 belongs to seeded owner, not our user
        var result = await controller.Details(1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_Get_UserRole_OtherPet_ReturnsForbid()
    {
        using var ctx = CreateContext();
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetsController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Delete(1);

        Assert.IsType<ForbidResult>(result);
    }
}
