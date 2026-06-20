using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;
using Moq;

namespace Dogtoralia.Tests.Maui.ViewModels;

public class ClinicsViewModelTests
{
    private static IReadOnlyList<ClinicDto> SeedClinics() =>
        Enumerable.Range(1, 3).Select(i => new ClinicDto
        {
            Id = i, Name = $"Clinic {i}", SpecialityName = "General",
            Phone = "1", Email = $"c{i}@x.com"
        }).ToList();

    [Fact]
    public async Task LoadAsync_PopulatesClinics()
    {
        var svc = new Mock<IClinicApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedClinics());
        var vm = new ClinicsViewModel(svc.Object);

        await vm.LoadAsync();

        Assert.Equal(3, vm.Clinics.Count);
        Assert.False(vm.IsBusy);
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task LoadAsync_OnException_SetsErrorMessage()
    {
        var svc = new Mock<IClinicApiService>();
        svc.Setup(s => s.GetAllAsync()).ThrowsAsync(new HttpRequestException("boom"));
        var vm = new ClinicsViewModel(svc.Object);

        await vm.LoadAsync();

        Assert.Empty(vm.Clinics);
        Assert.True(vm.HasError);
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_ClearsPreviousItemsBeforeReload()
    {
        var svc = new Mock<IClinicApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedClinics());
        var vm = new ClinicsViewModel(svc.Object);

        await vm.LoadAsync();
        await vm.LoadAsync();

        Assert.Equal(3, vm.Clinics.Count);
    }
}
