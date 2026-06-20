using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;
using Moq;

namespace Dogtoralia.Tests.Maui.ViewModels;

public class PetsViewModelTests
{
    private static IReadOnlyList<PetDto> SeedPets() =>
        Enumerable.Range(1, 3).Select(i => new PetDto
        {
            Id = i, Name = $"Pet {i}", Species = "Perro",
            PetOwnerId = i, PetOwnerName = $"Owner {i}"
        }).ToList();

    [Fact]
    public async Task LoadAsync_PopulatesPets()
    {
        var svc = new Mock<IPetApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());
        var vm = new PetsViewModel(svc.Object);

        await vm.LoadAsync();

        Assert.Equal(3, vm.Pets.Count);
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task DeleteAsync_RemovesPetAndCallsService()
    {
        var svc = new Mock<IPetApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());
        svc.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var vm = new PetsViewModel(svc.Object);
        await vm.LoadAsync();

        var target = vm.Pets.First();
        await vm.DeleteAsync(target);

        svc.Verify(s => s.DeleteAsync(target.Id), Times.Once);
        Assert.DoesNotContain(target, vm.Pets);
        Assert.Equal(2, vm.Pets.Count);
    }

    [Fact]
    public async Task DeleteAsync_NullPet_DoesNothing()
    {
        var svc = new Mock<IPetApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());
        var vm = new PetsViewModel(svc.Object);
        await vm.LoadAsync();

        await vm.DeleteAsync(null);

        svc.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        Assert.Equal(3, vm.Pets.Count);
    }

    [Fact]
    public async Task DeleteAsync_OnException_KeepsPetAndSetsError()
    {
        var svc = new Mock<IPetApiService>();
        svc.Setup(s => s.GetAllAsync()).ReturnsAsync(SeedPets());
        svc.Setup(s => s.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new HttpRequestException("409"));
        var vm = new PetsViewModel(svc.Object);
        await vm.LoadAsync();

        var target = vm.Pets.First();
        await vm.DeleteAsync(target);

        Assert.Contains(target, vm.Pets);
        Assert.True(vm.HasError);
    }
}
