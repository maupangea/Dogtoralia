using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class PetOwnersControllerTests
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
    public async Task Index_ReturnsAllOwnersWithPets()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Index() as ViewResult;
        var owners = result!.Model as IEnumerable<PetOwner>;

        Assert.Equal(10, owners!.Count());
    }

    // ── Details ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Details_ValidId_ReturnsViewModelWithPets()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Details(1) as ViewResult;
        var vm = result!.Model as PetOwnerDetailsViewModel;

        Assert.NotNull(vm);
        Assert.Equal(1, vm!.Owner.Id);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_IncludesPetsForOwner()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Details(1) as ViewResult;
        var vm = result!.Model as PetOwnerDetailsViewModel;

        // Seed assigns one pet per owner
        Assert.NotEmpty(vm!.Pets);
    }

    // ── Create GET ───────────────────────────────────────────────────────────

    [Fact]
    public void Create_Get_ReturnsEmptyForm()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = controller.Create() as ViewResult;
        var vm = result!.Model as PetOwnerFormViewModel;

        Assert.NotNull(vm);
        Assert.Null(vm!.Id);
    }

    // ── Create POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Post_ValidModel_SavesAndRedirectsToIndex()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var vm = new PetOwnerFormViewModel
        {
            Name = "New Owner",
            Email = "newowner@test.com",
            Phone = "+52-55-0000-0001"
        };

        var result = await controller.Create(vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(11, ctx.PetOwners.Count());
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new PetOwnerFormViewModel());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_DuplicateEmail_ReturnsViewWithError()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var vm = new PetOwnerFormViewModel
        {
            Name = "Duplicate",
            Email = "jorge.sanchez@gmail.com", // already in seed
            Phone = "+52-55-0000-0001"
        };

        var result = await controller.Create(vm) as ViewResult;

        Assert.NotNull(result);
        Assert.False(controller.ModelState.IsValid);
    }

    // ── Edit GET ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as PetOwnerFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Edit POST ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirectsToIndex()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var vm = new PetOwnerFormViewModel
        {
            Id = 1,
            Name = "Jorge Updated",
            Email = "jorge.sanchez@gmail.com",
            Phone = "+52-55-0000-9999"
        };

        var result = await controller.Edit(1, vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Jorge Updated", ctx.PetOwners.Find(1)!.Name);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var vm = new PetOwnerFormViewModel { Id = 5 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        controller.ModelState.AddModelError("Name", "Required");
        var vm = new PetOwnerFormViewModel { Id = 1 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Post_DuplicateEmailOnOtherOwner_ReturnsViewWithError()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);
        var vm = new PetOwnerFormViewModel
        {
            Id = 1,
            Name = "Jorge Sánchez",
            Email = "maria.fernandez@outlook.com", // belongs to owner Id=2
            Phone = "+52-55-1234-5601"
        };

        var result = await controller.Edit(1, vm) as ViewResult;

        Assert.NotNull(result);
        Assert.False(controller.ModelState.IsValid);
    }

    // ── Delete GET ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsOwner()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<PetOwner>(result!.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new PetOwnersController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Delete POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteConfirmed_OwnerWithNoPets_DeletesAndRedirects()
    {
        using var ctx = CreateContext();
        // Add an owner with no pets to safely delete
        var owner = new PetOwner { Name = "Temp", Email = "temp@test.com", Phone = "000", CreatedAt = DateTime.UtcNow };
        ctx.PetOwners.Add(owner);
        await ctx.SaveChangesAsync();

        var controller = new PetOwnersController(ctx);
        var result = await controller.DeleteConfirmed(owner.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(await ctx.PetOwners.FindAsync(owner.Id));
    }
}
