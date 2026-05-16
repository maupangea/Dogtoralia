using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class PetsOwnershipTests
{
    private const string UserOwnerId = "user-pets-id";
    private const int UserLinkedOwnerId = 99;

    private static PetOwnerDto UserOwner => new()
    {
        Id = UserLinkedOwnerId, Name = "User Owner", Email = "userowner@example.com",
        Phone = "1", UserId = UserOwnerId, PetCount = 1, CreatedAt = DateTime.UtcNow
    };

    private static PetDto UserPet => new()
    {
        Id = 50, Name = "MyPet", Species = "Perro", Breed = "Labrador",
        PetOwnerId = UserLinkedOwnerId, PetOwnerName = "User Owner",
        PetOwnerEmail = "userowner@example.com", PetOwnerPhone = "1",
        DateOfBirth = DateTime.UtcNow.AddYears(-2), CreatedAt = DateTime.UtcNow
    };

    private static PetDto OtherPet => new()
    {
        Id = 1, Name = "Other Pet", Species = "Gato", Breed = "Mix",
        PetOwnerId = 1, PetOwnerName = "Other Owner",
        PetOwnerEmail = "other@x.com", PetOwnerPhone = "1",
        DateOfBirth = DateTime.UtcNow.AddYears(-1), CreatedAt = DateTime.UtcNow
    };

    private static PetsController CreateUserController(
        Mock<IPetService> petSvc, Mock<IPetOwnerService> ownerSvc)
    {
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetOwnerDto> { UserOwner });
        var controller = new PetsController(petSvc.Object, ownerSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetRegularUser(controller, mockUm, UserOwnerId);
        return controller;
    }

    [Fact]
    public async Task Index_UserRole_ReturnsOnlyOwnPets()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetDto> { UserPet, OtherPet });
        var ownerSvc = new Mock<IPetOwnerService>();
        var controller = CreateUserController(petSvc, ownerSvc);

        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.All(vm!.Pets, p => Assert.Equal(UserLinkedOwnerId, p.PetOwnerId));
        Assert.Equal(1, vm.Pets.Count);
    }

    [Fact]
    public async Task Create_Get_UserRole_OtherOwner_ReturnsForbid()
    {
        var petSvc = new Mock<IPetService>();
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new PetOwnerDto { Id = 1 });
        var controller = CreateUserController(petSvc, ownerSvc);

        var result = await controller.Create(petOwnerId: 1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OwnPet_ReturnsView()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(UserPet.Id)).ReturnsAsync(UserPet);
        var ownerSvc = new Mock<IPetOwnerService>();
        var controller = CreateUserController(petSvc, ownerSvc);

        var result = await controller.Details(UserPet.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_UserRole_OtherPet_ReturnsForbid()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(OtherPet.Id)).ReturnsAsync(OtherPet);
        var ownerSvc = new Mock<IPetOwnerService>();
        var controller = CreateUserController(petSvc, ownerSvc);

        Assert.IsType<ForbidResult>(await controller.Details(OtherPet.Id));
    }

    [Fact]
    public async Task Delete_Get_UserRole_OtherPet_ReturnsForbid()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(OtherPet.Id)).ReturnsAsync(OtherPet);
        var ownerSvc = new Mock<IPetOwnerService>();
        var controller = CreateUserController(petSvc, ownerSvc);

        Assert.IsType<ForbidResult>(await controller.Delete(OtherPet.Id));
    }
}
