using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.Tests.Helpers;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class PetOwnersOwnershipTests
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

    private const string UserOwnerId = "user-owner-id";

    private static async Task<PetOwner> AddOwnerWithUser(DogtoraliaDbContext ctx, string userId)
    {
        var owner = new PetOwner
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Phone = "+52-55-0000-9999",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        ctx.PetOwners.Add(owner);
        await ctx.SaveChangesAsync();
        return owner;
    }

    [Fact]
    public async Task Details_UserRole_OwnProfile_ReturnsView()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Details(owner.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OtherOwner_ReturnsForbid()
    {
        using var ctx = CreateContext();
        // Seeded owner Id=1 belongs to no user; logged-in user is "user-owner-id" with own owner
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        // Try to access seeded owner Id=1 (not theirs)
        var result = await controller.Details(1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Get_UserRole_OwnProfile_ReturnsView()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Edit(owner.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Get_UserRole_OtherOwner_ReturnsForbid()
    {
        using var ctx = CreateContext();
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var result = await controller.Edit(1); // seeded owner, not theirs

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Post_UserRole_OwnProfile_RedirectsToDetails()
    {
        using var ctx = CreateContext();
        var owner = await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var vm = new PetOwnerFormViewModel
        {
            Id = owner.Id,
            Name = "Updated Name",
            Email = "testuser@example.com",
            Phone = "+52-55-0000-9999"
        };

        var result = await controller.Edit(owner.Id, vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal(owner.Id, result.RouteValues!["id"]);
    }

    [Fact]
    public async Task Edit_Post_UserRole_OtherOwner_ReturnsForbid()
    {
        using var ctx = CreateContext();
        await AddOwnerWithUser(ctx, UserOwnerId);
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ctx, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);

        var vm = new PetOwnerFormViewModel { Id = 1, Name = "X", Email = "x@x.com", Phone = "000" };

        var result = await controller.Edit(1, vm);

        Assert.IsType<ForbidResult>(result);
    }
}
