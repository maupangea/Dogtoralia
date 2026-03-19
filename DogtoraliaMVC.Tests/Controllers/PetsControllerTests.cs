using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class PetsControllerTests
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

    // ── Index ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Index_NoFilter_ReturnsFirstPageAndCorrectTotalPages()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        // page size 8, 10 seeded → 8 on page 1, 2 total pages
        Assert.Equal(8, vm!.Pets.Count);
        Assert.Equal(2, vm.Pets.TotalPages);
    }

    [Fact]
    public async Task Index_FilterBySpecies_ReturnsOnlyThatSpecies()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Index("Perro", 1) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.All(vm!.Pets, p => Assert.Equal("Perro", p.Species));
    }

    [Fact]
    public async Task Index_Pagination_ReturnsCorrectPage()
    {
        using var ctx = CreateContext();
        // Add more pets to trigger pagination (page size = 8, 10 already seeded)
        ctx.Pets.Add(new Pet { Name = "X", Species = "Gato", Breed = "B", DateOfBirth = new DateTime(2021, 1, 1), OwnerName = "O", OwnerEmail = "o@o.com", OwnerPhone = "1", CreatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var controller = new PetsController(ctx);
        var result = await controller.Index(null, 2) as ViewResult;
        var vm = result!.Model as PetsIndexViewModel;

        Assert.Equal(2, vm!.Pets.PageIndex);
        Assert.Equal(3, vm.Pets.Count);
    }

    // ── Details ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Details_ValidId_ReturnsPet()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Details(1) as ViewResult;

        Assert.IsType<Pet>(result!.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Create GET ───────────────────────────────────────────────────────────

    [Fact]
    public void Create_Get_ReturnsViewWithSpeciesOptions()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = controller.Create() as ViewResult;
        var vm = result!.Model as PetFormViewModel;

        Assert.NotNull(vm!.SpeciesOptions);
    }

    // ── Create POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);
        var vm = new PetFormViewModel
        {
            Name = "Buddy",
            Species = "Dog",
            Breed = "Poodle",
            DateOfBirth = new DateTime(2022, 5, 1),
            OwnerName = "Jane Doe",
            OwnerEmail = "jane@test.com",
            OwnerPhone = "+52-55-0000-0000"
        };

        var result = await controller.Create(vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(11, ctx.Pets.Count());
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new PetFormViewModel());

        Assert.IsType<ViewResult>(result);
    }

    // ── Edit GET ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as PetFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Edit POST ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);
        var vm = new PetFormViewModel
        {
            Id = 1,
            Name = "Max Updated",
            Species = "Dog",
            Breed = "Labrador",
            DateOfBirth = new DateTime(2020, 3, 15),
            OwnerName = "Jorge",
            OwnerEmail = "jorge@test.com",
            OwnerPhone = "+52-55-0000-0000"
        };

        var result = await controller.Edit(1, vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Max Updated", ctx.Pets.Find(1)!.Name);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);
        var vm = new PetFormViewModel { Id = 5 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);
        controller.ModelState.AddModelError("Name", "Required");
        var vm = new PetFormViewModel { Id = 1 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<ViewResult>(result);
    }

    // ── Delete GET ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsPet()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<Pet>(result!.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Delete POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteConfirmed_ValidId_DeletesAndRedirects()
    {
        using var ctx = CreateContext();
        var controller = new PetsController(ctx);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(ctx.Pets.Find(1));
    }
}
