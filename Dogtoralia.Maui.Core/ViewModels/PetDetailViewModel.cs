using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class PetDetailViewModel : BaseViewModel
{
    private readonly IPetApiService _petService;

    [ObservableProperty]
    private PetDto? _pet;

    public PetDetailViewModel(IPetApiService petService)
    {
        _petService = petService;
        Title = "Detalle de mascota";
    }

    [RelayCommand]
    public async Task LoadAsync(int id)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            Pet = await _petService.GetByIdAsync(id);
            if (Pet is not null)
                Title = Pet.Name;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo cargar la mascota: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
