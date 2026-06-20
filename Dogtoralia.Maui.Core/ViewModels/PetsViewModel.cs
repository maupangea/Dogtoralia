using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class PetsViewModel : BaseViewModel
{
    private readonly IPetApiService _petService;

    public ObservableCollection<PetDto> Pets { get; } = new();

    public PetsViewModel(IPetApiService petService)
    {
        _petService = petService;
        Title = "Mascotas";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var pets = await _petService.GetAllAsync();
            Pets.Clear();
            foreach (var pet in pets)
                Pets.Add(pet);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudieron cargar las mascotas: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAsync(PetDto? pet)
    {
        if (pet is null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await _petService.DeleteAsync(pet.Id);
            Pets.Remove(pet);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo eliminar la mascota: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
