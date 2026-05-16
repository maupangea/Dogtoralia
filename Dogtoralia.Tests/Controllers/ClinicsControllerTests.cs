using Dogtoralia.MVC.Controllers;
using Dogtoralia.MVC.Services;
using Dogtoralia.MVC.ViewModels;
using Dogtoralia.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Dogtoralia.Tests.Controllers;

public class ClinicsControllerTests
{
    private static IEnumerable<ClinicDto> SeedClinics() =>
        Enumerable.Range(1, 6).Select(i => new ClinicDto
        {
            Id = i, Name = $"Clinic {i}", Address = "A", Phone = "1", Email = $"c{i}@x.com",
            SpecialityId = i <= 2 ? 1 : i, SpecialityName = "General", VeterinarianCount = 1,
            CreatedAt = DateTime.UtcNow
        });

    private static ClinicsController CreateController(
        Mock<IClinicService>? clinicSvc = null,
        Mock<ISpecialityService>? spSvc = null,
        Mock<IVeterinarianService>? vetSvc = null,
        Mock<IAppointmentService>? apptSvc = null)
    {
        clinicSvc ??= new Mock<IClinicService>();
        spSvc ??= new Mock<ISpecialityService>();
        vetSvc ??= new Mock<IVeterinarianService>();
        apptSvc ??= new Mock<IAppointmentService>();

        clinicSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedClinics());
        spSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SpecialityDto>());
        vetSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<VeterinarianDto>());
        apptSvc.Setup(s => s.GetAllAsync(It.IsAny<int?>(), It.IsAny<Dogtoralia.Shared.Models.AppointmentStatus?>()))
               .ReturnsAsync(new List<AppointmentDto>());

        return new ClinicsController(clinicSvc.Object, spSvc.Object, vetSvc.Object, apptSvc.Object);
    }

    [Fact]
    public async Task Index_NoFilter_ReturnsAllClinics()
    {
        var controller = CreateController();
        var result = await controller.Index(null, 1) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.NotNull(vm);
        Assert.Equal(6, vm!.Clinics.Count);
    }

    [Fact]
    public async Task Index_FilterBySpeciality_ReturnsClinicsForThatSpeciality()
    {
        var controller = CreateController();
        var result = await controller.Index(1, 1) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.All(vm!.Clinics, c => Assert.Equal(1, c.SpecialityId));
    }

    [Fact]
    public async Task Index_Pagination_ReturnsCorrectPage()
    {
        var clinicSvc = new Mock<IClinicService>();
        var controller = CreateController(clinicSvc);
        var clinics = Enumerable.Range(1, 9).Select(i => new ClinicDto
        {
            Id = i, Name = $"Clinic {i}", Address = "A", Phone = "1", Email = $"c{i}@x.com",
            SpecialityId = 1, SpecialityName = "G", CreatedAt = DateTime.UtcNow
        }).ToList();
        clinicSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(clinics);
        var result = await controller.Index(null, 2) as ViewResult;
        var vm = result!.Model as ClinicsIndexViewModel;

        Assert.Equal(2, vm!.Clinics.PageIndex);
        Assert.Equal(3, vm.Clinics.Count);
    }

    [Fact]
    public async Task Details_ValidId_ReturnsClinicDetailsViewModel()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedClinics().First());
        var controller = CreateController(clinicSvc);
        var result = await controller.Details(1) as ViewResult;

        Assert.NotNull(result);
        Assert.IsType<ClinicDetailsViewModel>(result!.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ClinicDto?)null);
        var controller = CreateController(clinicSvc);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithSpecialityOptions()
    {
        var spSvc = new Mock<ISpecialityService>();
        spSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SpecialityDto>
        {
            new() { Id = 1, Name = "General" }
        });
        var controller = CreateController(spSvc: spSvc);
        var result = await controller.Create() as ViewResult;
        var vm = result!.Model as ClinicFormViewModel;

        Assert.NotNull(vm!.SpecialityOptions);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.CreateAsync(It.IsAny<ClinicWriteDto>()))
                 .ReturnsAsync(new ClinicDto { Id = 7, Name = "Test Clinic" });
        var controller = CreateController(clinicSvc);
        var vm = new ClinicFormViewModel
        {
            Name = "Test Clinic", Address = "123 St", Phone = "+52-55-0000-0000",
            Email = "test@clinic.mx", SpecialityId = 1
        };

        var result = await controller.Create(vm);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new ClinicFormViewModel());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewModel()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedClinics().First());
        var controller = CreateController(clinicSvc);
        var result = await controller.Edit(1) as ViewResult;
        var vm = result!.Model as ClinicFormViewModel;

        Assert.Equal(1, vm!.Id);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ClinicDto?)null);
        var controller = CreateController(clinicSvc);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.UpdateAsync(1, It.IsAny<ClinicWriteDto>())).ReturnsAsync(true);
        var controller = CreateController(clinicSvc);
        var vm = new ClinicFormViewModel
        {
            Id = 1, Name = "Updated", Address = "X", Phone = "1", Email = "u@x.com", SpecialityId = 1
        };

        var result = await controller.Edit(1, vm);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        var controller = CreateController();
        var result = await controller.Edit(1, new ClinicFormViewModel { Id = 2 });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Edit(1, new ClinicFormViewModel { Id = 1 });

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsClinicDto()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(SeedClinics().First());
        var controller = CreateController(clinicSvc);
        var result = await controller.Delete(1) as ViewResult;

        Assert.IsType<ClinicDto>(result!.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ClinicDto?)null);
        var controller = CreateController(clinicSvc);

        Assert.IsType<NotFoundResult>(await controller.Delete(999));
    }

    [Fact]
    public async Task DeleteConfirmed_ValidId_DeletesAndRedirects()
    {
        var clinicSvc = new Mock<IClinicService>();
        clinicSvc.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var controller = CreateController(clinicSvc);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        clinicSvc.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
