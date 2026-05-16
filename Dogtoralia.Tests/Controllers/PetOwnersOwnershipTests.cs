using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class PetOwnersOwnershipTests
{
    private const string UserOwnerId = "user-owner-id";
    private const int UserLinkedOwnerId = 99;

    private static PetOwnerDto UserOwner => new()
    {
        Id = UserLinkedOwnerId, Name = "Test User", Email = "testuser@example.com",
        Phone = "+52-55-0000-9999", UserId = UserOwnerId, PetCount = 0, CreatedAt = DateTime.UtcNow
    };

    private static PetOwnerDto OtherOwner => new()
    {
        Id = 1, Name = "Other Owner", Email = "other@example.com",
        Phone = "1", UserId = null, PetCount = 0, CreatedAt = DateTime.UtcNow
    };

    private static PetOwnersController CreateUserController(
        Mock<IPetOwnerService> ownerSvc, Mock<IPetService>? petSvc = null)
    {
        petSvc ??= new Mock<IPetService>();
        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetDto>());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetOwnerDto> { UserOwner, OtherOwner });

        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var controller = new PetOwnersController(ownerSvc.Object, petSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);
        return controller;
    }

    [Fact]
    public async Task Details_UserRole_OwnProfile_ReturnsView()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(UserLinkedOwnerId)).ReturnsAsync(UserOwner);
        var controller = CreateUserController(ownerSvc);

        var result = await controller.Details(UserLinkedOwnerId);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OtherOwner_ReturnsForbid()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(OtherOwner.Id)).ReturnsAsync(OtherOwner);
        var controller = CreateUserController(ownerSvc);

        var result = await controller.Details(OtherOwner.Id);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Get_UserRole_OwnProfile_ReturnsView()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(UserLinkedOwnerId)).ReturnsAsync(UserOwner);
        var controller = CreateUserController(ownerSvc);

        var result = await controller.Edit(UserLinkedOwnerId);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Get_UserRole_OtherOwner_ReturnsForbid()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(OtherOwner.Id)).ReturnsAsync(OtherOwner);
        var controller = CreateUserController(ownerSvc);

        var result = await controller.Edit(OtherOwner.Id);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Post_UserRole_OwnProfile_RedirectsToDetails()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(UserLinkedOwnerId)).ReturnsAsync(UserOwner);
        ownerSvc.Setup(s => s.UpdateAsync(UserLinkedOwnerId, It.IsAny<PetOwnerWriteDto>())).ReturnsAsync(true);
        var controller = CreateUserController(ownerSvc);

        var vm = new PetOwnerFormViewModel
        {
            Id = UserLinkedOwnerId, Name = "Updated Name",
            Email = "testuser@example.com", Phone = "+52-55-0000-9999"
        };

        var result = await controller.Edit(UserLinkedOwnerId, vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal(UserLinkedOwnerId, result.RouteValues!["id"]);
    }

    [Fact]
    public async Task Edit_Post_UserRole_OtherOwner_ReturnsForbid()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(OtherOwner.Id)).ReturnsAsync(OtherOwner);
        var controller = CreateUserController(ownerSvc);

        var vm = new PetOwnerFormViewModel { Id = OtherOwner.Id, Name = "X", Email = "x@x.com", Phone = "000" };

        var result = await controller.Edit(OtherOwner.Id, vm);

        Assert.IsType<ForbidResult>(result);
    }
}
