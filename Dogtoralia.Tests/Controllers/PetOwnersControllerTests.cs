using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class PetOwnersControllerTests
{
    private static IEnumerable<PetOwnerDto> SeedOwners() =>
        Enumerable.Range(1, 10).Select(i => new PetOwnerDto
        {
            Id = i, Name = $"Owner {i}", Email = $"o{i}@x.com", Phone = "1",
            PetCount = 0, CreatedAt = DateTime.UtcNow
        });

    private static IEnumerable<PetDto> SeedPets() =>
        Enumerable.Range(1, 10).Select(i => new PetDto
        {
            Id = i, Name = $"Pet {i}", Species = "Perro", Breed = "Mix",
            PetOwnerId = i, PetOwnerName = $"Owner {i}",
            PetOwnerEmail = $"o{i}@x.com", PetOwnerPhone = "1",
            DateOfBirth = DateTime.UtcNow.AddYears(-1), CreatedAt = DateTime.UtcNow
        });

    private static PetOwnersController CreateController(
        Mock<IPetOwnerService>? ownerSvc = null,
        Mock<IPetService>? petSvc = null,
        Mock<UserManager<IdentityUser>>? mockUm = null)
    {
        ownerSvc ??= new Mock<IPetOwnerService>();
        petSvc ??= new Mock<IPetService>();
        mockUm ??= ControllerTestHelpers.CreateMockUserManager();

        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());

        var controller = new PetOwnersController(ownerSvc.Object, petSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetAdminUser(controller);
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsAllOwners()
    {
        var controller = CreateController();
        var result = await controller.Index() as ViewResult;
        var owners = result!.Model as IEnumerable<PetOwnerDto>;

        Assert.Equal(10, owners!.Count());
    }

    [Fact]
    public async Task Details_ValidId_ReturnsViewModelWithPets()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);
        var result = await controller.Details(1) as ViewResult;
        var vm = result!.Model as PetOwnerDetailsViewModel;

        Assert.NotNull(vm);
        Assert.Equal(1, vm!.Owner.Id);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetOwnerDto?)null);
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);

        Assert.IsType<NotFoundResult>(await controller.Details(999));
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var controller = CreateController();

        Assert.IsType<ViewResult>(controller.Create());
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PetOwnerDto>());
        ownerSvc.Setup(s => s.CreateAsync(It.IsAny<PetOwnerWriteDto>())).ReturnsAsync(new PetOwnerDto { Id = 11 });
        var controller = CreateController(ownerSvc);
        var vm = new PetOwnerFormViewModel { Name = "New Owner", Email = "new@x.com", Phone = "1" };

        var result = await controller.Create(vm);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Create_Post_DuplicateEmail_ReturnsView()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);
        var vm = new PetOwnerFormViewModel { Name = "X", Email = "o1@x.com", Phone = "1" };

        var result = await controller.Create(vm);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);
        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as PetOwnerFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetOwnerDto?)null);
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);

        Assert.IsType<NotFoundResult>(await controller.Edit(999));
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        ownerSvc.Setup(s => s.UpdateAsync(1, It.IsAny<PetOwnerWriteDto>())).ReturnsAsync(true);
        var controller = CreateController(ownerSvc);
        var vm = new PetOwnerFormViewModel { Id = 1, Name = "Updated", Email = "updated@x.com", Phone = "1" };

        Assert.IsType<RedirectToActionResult>(await controller.Edit(1, vm));
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        var controller = CreateController();

        Assert.IsType<BadRequestResult>(await controller.Edit(1, new PetOwnerFormViewModel { Id = 2 }));
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsPetOwnerDto()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);
        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<PetOwnerDto>(result!.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetOwnerDto?)null);
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);

        Assert.IsType<NotFoundResult>(await controller.Delete(999));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNoPets_DeletesAndRedirects()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        ownerSvc.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var controller = CreateController(ownerSvc);

        Assert.IsType<RedirectToActionResult>(await controller.DeleteConfirmed(1));
    }

    [Fact]
    public async Task DeleteConfirmed_WithPets_ReturnsView()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        var ownerWithPets = new PetOwnerDto { Id = 1, Name = "Owner", Email = "o@x.com", Phone = "1", PetCount = 3 };
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(ownerWithPets);
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());
        var controller = CreateController(ownerSvc);

        Assert.IsType<ViewResult>(await controller.DeleteConfirmed(1));
    }
}
