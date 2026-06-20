using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;
using Moq;

namespace Dogtoralia.Tests.Maui.ViewModels;

public class PetEditViewModelTests
{
    private static IReadOnlyList<PetOwnerDto> SeedOwners() =>
        Enumerable.Range(1, 3).Select(i => new PetOwnerDto
        {
            Id = i, Name = $"Owner {i}", Email = $"o{i}@x.com", Phone = "1"
        }).ToList();

    private static (PetEditViewModel vm, Mock<IPetApiService> pet, Mock<IPetOwnerApiService> owner) Create()
    {
        var pet = new Mock<IPetApiService>();
        var owner = new Mock<IPetOwnerApiService>();
        owner.Setup(o => o.GetAllAsync()).ReturnsAsync(SeedOwners());
        return (new PetEditViewModel(pet.Object, owner.Object), pet, owner);
    }

    [Fact]
    public async Task LoadAsync_New_PopulatesOwnersAndIsNotEdit()
    {
        var (vm, _, _) = Create();

        await vm.LoadAsync(0);

        Assert.Equal(3, vm.Owners.Count);
        Assert.False(vm.IsEdit);
    }

    [Fact]
    public async Task LoadAsync_Edit_PopulatesFieldsFromPet()
    {
        var (vm, pet, _) = Create();
        pet.Setup(p => p.GetByIdAsync(5)).ReturnsAsync(new PetDto
        {
            Id = 5, Name = "Rex", Species = "Perro", Breed = "Mix",
            DateOfBirth = new DateTime(2019, 5, 1), Notes = "n", PetOwnerId = 2
        });

        await vm.LoadAsync(5);

        Assert.True(vm.IsEdit);
        Assert.Equal("Rex", vm.Name);
        Assert.Equal("Perro", vm.SelectedSpecies);
        Assert.Equal(2, vm.SelectedOwner!.Id);
    }

    [Fact]
    public async Task SaveAsync_Create_CallsCreateWithMappedDtoAndRaisesSaved()
    {
        var (vm, pet, _) = Create();
        await vm.LoadAsync(0);
        vm.Name = "Bobby";
        vm.SelectedSpecies = "Gato";
        vm.Breed = "Siames";
        vm.DateOfBirth = new DateTime(2021, 2, 3);
        vm.SelectedOwner = vm.Owners.First(o => o.Id == 2);

        PetWriteDto? captured = null;
        pet.Setup(p => p.CreateAsync(It.IsAny<PetWriteDto>()))
            .Callback<PetWriteDto>(d => captured = d)
            .ReturnsAsync(new PetDto { Id = 99 });

        var raised = false;
        vm.Saved += (_, _) => raised = true;

        await vm.SaveAsync();

        pet.Verify(p => p.CreateAsync(It.IsAny<PetWriteDto>()), Times.Once);
        Assert.NotNull(captured);
        Assert.Equal("Bobby", captured!.Name);
        Assert.Equal("Gato", captured.Species);
        Assert.Equal(2, captured.PetOwnerId);
        Assert.True(raised);
    }

    [Fact]
    public async Task SaveAsync_Edit_CallsUpdate()
    {
        var (vm, pet, _) = Create();
        pet.Setup(p => p.GetByIdAsync(7)).ReturnsAsync(new PetDto
        {
            Id = 7, Name = "Old", Species = "Perro", PetOwnerId = 1
        });
        await vm.LoadAsync(7);
        vm.Name = "Updated";

        await vm.SaveAsync();

        pet.Verify(p => p.UpdateAsync(7, It.Is<PetWriteDto>(d => d.Name == "Updated")), Times.Once);
        pet.Verify(p => p.CreateAsync(It.IsAny<PetWriteDto>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_MissingOwner_SetsErrorAndDoesNotCallService()
    {
        var (vm, pet, _) = Create();
        await vm.LoadAsync(0);
        vm.Name = "NoOwner";
        vm.SelectedSpecies = "Perro";
        vm.SelectedOwner = null;

        await vm.SaveAsync();

        Assert.True(vm.HasError);
        pet.Verify(p => p.CreateAsync(It.IsAny<PetWriteDto>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_MissingName_SetsErrorAndDoesNotCallService()
    {
        var (vm, pet, _) = Create();
        await vm.LoadAsync(0);
        vm.Name = "  ";
        vm.SelectedSpecies = "Perro";
        vm.SelectedOwner = vm.Owners.First();

        await vm.SaveAsync();

        Assert.True(vm.HasError);
        pet.Verify(p => p.CreateAsync(It.IsAny<PetWriteDto>()), Times.Never);
    }
}
