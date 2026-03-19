using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Tests.Controllers;

public class ClinicsControllerTests
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
    public async Task Index_NoFilter_ReturnsAllClinics()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.NotNull(vm);
        Assert.Equal(6, vm!.Clinics.Count);
    }

    [Fact]
    public async Task Index_FilterBySpeciality_ReturnsClinicsForThatSpeciality()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        // Speciality 1 (General Practice) has 2 clinics in seed
        var result = await controller.Index(1, 1) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.All(vm!.Clinics, c => Assert.Equal(1, c.SpecialityId));
    }

    [Fact]
    public async Task Index_Pagination_ReturnsCorrectPage()
    {
        using var ctx = CreateContext();
        // Add more clinics to trigger pagination (page size = 6)
        var sp = ctx.Specialities.First();
        for (int i = 0; i < 3; i++)
            ctx.Clinics.Add(new Clinic { Name = $"Extra {i}", Address = "A", Phone = "1", Email = $"e{i}@x.com", SpecialityId = sp.Id, CreatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var controller = new ClinicsController(ctx);
        var result = await controller.Index(null, 2) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.Equal(2, vm!.Clinics.PageIndex);
        Assert.Equal(3, vm.Clinics.Count);
    }

    // ── Details ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Details_ValidId_ReturnsClinic()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Details(1) as ViewResult;

        Assert.NotNull(result);
        Assert.IsType<Clinic>(result!.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Create GET ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Get_ReturnsViewWithSpecialityOptions()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Create() as ViewResult;
        var vm = result!.Model as ClinicFormViewModel;

        Assert.NotNull(vm!.SpecialityOptions);
    }

    // ── Create POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);
        var vm = new ClinicFormViewModel
        {
            Name = "Test Clinic",
            Address = "123 St",
            Phone = "+52-55-0000-0000",
            Email = "test@clinic.mx",
            SpecialityId = 1
        };

        var result = await controller.Create(vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(7, ctx.Clinics.Count());
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new ClinicFormViewModel());

        Assert.IsType<ViewResult>(result);
    }

    // ── Edit GET ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as ClinicFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Edit POST ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);
        var vm = new ClinicFormViewModel
        {
            Id = 1,
            Name = "Updated Name",
            Address = "New Address",
            Phone = "+52-55-0000-0000",
            Email = "updated@clinic.mx",
            SpecialityId = 1
        };

        var result = await controller.Edit(1, vm);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Updated Name", ctx.Clinics.Find(1)!.Name);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);
        var vm = new ClinicFormViewModel { Id = 2 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);
        controller.ModelState.AddModelError("Name", "Required");
        var vm = new ClinicFormViewModel { Id = 1 };

        var result = await controller.Edit(1, vm);

        Assert.IsType<ViewResult>(result);
    }

    // ── Delete GET ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsClinic()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<Clinic>(result!.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ── Delete POST ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteConfirmed_ValidId_DeletesAndRedirects()
    {
        using var ctx = CreateContext();
        var controller = new ClinicsController(ctx);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(ctx.Clinics.Find(1));
    }
}
