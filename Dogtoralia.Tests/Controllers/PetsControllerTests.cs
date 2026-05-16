using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Dogtoralia.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class PetsControllerTests
{
    private static IEnumerable<PetOwnerDto> SeedOwners() =>
        Enumerable.Range(1, 10).Select(i => new PetOwnerDto
        {
            Id = i, Name = $"Owner {i}", Email = $"o{i}@x.com", Phone = "1",
            PetCount = 1, CreatedAt = DateTime.UtcNow
        });

    private static IEnumerable<PetDto> SeedPets() =>
        Enumerable.Range(1, 10).Select(i => new PetDto
        {
            Id = i, Name = $"Pet {i}", Species = i % 2 == 0 ? "Gato" : "Perro",
            Breed = "Mix", PetOwnerId = i, PetOwnerName = $"Owner {i}",
            PetOwnerEmail = $"o{i}@x.com", PetOwnerPhone = "1",
            DateOfBirth = DateTime.UtcNow.AddYears(-2), CreatedAt = DateTime.UtcNow
        });

    private static PetsController CreateController(
        Mock<IPetService>? petSvc = null,
        Mock<IPetOwnerService>? ownerSvc = null,
        Mock<UserManager<IdentityUser>>? mockUm = null)
    {
        petSvc ??= new Mock<IPetService>();
        ownerSvc ??= new Mock<IPetOwnerService>();
        mockUm ??= ControllerTestHelpers.CreateMockUserManager();

        petSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());
        ownerSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedOwners());

        var controller = new PetsController(petSvc.Object, ownerSvc.Object, mockUm.Object);
        ControllerTestHelpers.SetAdminUser(controller);
        return controller;
    }

    [Fact]
    public async Task Index_NoFilter_ReturnsFirstPageAndCorrectTotalPages()
    {
        var controller = CreateController();
        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.Equal(8, vm!.Pets.Count);
        Assert.Equal(2, vm.Pets.TotalPages);
    }

    [Fact]
    public async Task Index_FilterBySpecies_ReturnsOnlyThatSpecies()
    {
        var controller = CreateController();
        var result = await controller.Index("Perro", 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.All(vm!.Pets, p => Assert.Equal("Perro", p.Species));
    }

    [Fact]
    public async Task Details_ValidId_ReturnsPetDto()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedPets().First());
        var controller = CreateController(petSvc);
        var result = await controller.Details(1) as ViewResult;

        Assert.IsType<PetDto>(result!.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetDto?)null);
        var controller = CreateController(petSvc);

        Assert.IsType<NotFoundResult>(await controller.Details(999));
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithOwnerData()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        var controller = CreateController(ownerSvc: ownerSvc);
        var result = await controller.Create(1) as ViewResult;
        var vm = result!.Model as PetFormViewModel;

        Assert.Equal(1, vm!.PetOwnerId);
    }

    [Fact]
    public async Task Create_Get_InvalidOwner_ReturnsNotFound()
    {
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetOwnerDto?)null);
        var controller = CreateController(ownerSvc: ownerSvc);

        Assert.IsType<NotFoundResult>(await controller.Create(999));
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToPetOwnerDetails()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.CreateAsync(It.IsAny<PetWriteDto>())).ReturnsAsync(new PetDto { Id = 11 });
        var ownerSvc = new Mock<IPetOwnerService>();
        ownerSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedOwners().First());
        var controller = CreateController(petSvc, ownerSvc);
        var vm = new PetFormViewModel
        {
            Name = "Buddy", Species = "Perro", Breed = "Beagle",
            DateOfBirth = DateTime.Today.AddYears(-1), PetOwnerId = 1
        };

        var result = await controller.Create(vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal("PetOwners", result.ControllerName);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedPets().First());
        var controller = CreateController(petSvc);
        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as PetFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PetDto?)null);
        var controller = CreateController(petSvc);

        Assert.IsType<NotFoundResult>(await controller.Edit(999));
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToPetOwnerDetails()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedPets().First());
        petSvc.Setup(s => s.UpdateAsync(1, It.IsAny<PetWriteDto>())).ReturnsAsync(true);
        var controller = CreateController(petSvc);
        var vm = new PetFormViewModel
        {
            Id = 1, Name = "Updated", Species = "Gato", Breed = "Mix",
            DateOfBirth = DateTime.Today.AddYears(-1), PetOwnerId = 1
        };

        var result = await controller.Edit(1, vm) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal("PetOwners", result.ControllerName);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        var controller = CreateController();

        Assert.IsType<BadRequestResult>(await controller.Edit(1, new PetFormViewModel { Id = 2 }));
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsPetDto()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedPets().First());
        var controller = CreateController(petSvc);
        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<PetDto>(result!.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidId_DeletesAndRedirects()
    {
        var petSvc = new Mock<IPetService>();
        petSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedPets().First());
        petSvc.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var controller = CreateController(petSvc);

        var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

        Assert.Equal("Details", result!.ActionName);
        Assert.Equal("PetOwners", result.ControllerName);
    }
}
