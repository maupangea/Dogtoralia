using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;
using Moq;

namespace Dogtoralia.Tests.Maui.ViewModels;

public class VeterinariansViewModelTests
{
    private static IReadOnlyList<VeterinarianDto> SeedVets() =>
        Enumerable.Range(1, 4).Select(i => new VeterinarianDto
        {
            Id = i, FirstName = $"Vet{i}", LastName = "Doe",
            FullName = $"Vet{i} Doe", ClinicName = "Clinic", Email = $"v{i}@x.com"
        }).ToList();

    [Fact]
    public async Task LoadAsync_PopulatesVeterinarians()
    {
        var svc = new Mock<IVeterinarianApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedVets());
        var vm = new VeterinariansViewModel(svc.Object);

        await vm.LoadAsync();

        Assert.Equal(4, vm.Veterinarians.Count);
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task LoadAsync_OnException_SetsErrorMessage()
    {
        var svc = new Mock<IVeterinarianApiService>();
        svc.Setup(s => s.GetAllAsync()).ThrowsAsync(new HttpRequestException("down"));
        var vm = new VeterinariansViewModel(svc.Object);

        await vm.LoadAsync();

        Assert.True(vm.HasError);
        Assert.Empty(vm.Veterinarians);
    }
}
