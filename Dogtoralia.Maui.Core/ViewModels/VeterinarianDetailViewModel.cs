using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class VeterinarianDetailViewModel : BaseViewModel
{
    private readonly IVeterinarianApiService _veterinarianService;

    [ObservableProperty]
    private VeterinarianDto? _veterinarian;

    public VeterinarianDetailViewModel(IVeterinarianApiService veterinarianService)
    {
        _veterinarianService = veterinarianService;
        Title = "Detalle de veterinario";
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
            Veterinarian = await _veterinarianService.GetByIdAsync(id);
            if (Veterinarian is not null)
                Title = Veterinarian.FullName;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo cargar el veterinario: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
