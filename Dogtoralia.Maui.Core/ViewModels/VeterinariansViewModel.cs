using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class VeterinariansViewModel : BaseViewModel
{
    private readonly IVeterinarianApiService _veterinarianService;

    public ObservableCollection<VeterinarianDto> Veterinarians { get; } = new();

    public VeterinariansViewModel(IVeterinarianApiService veterinarianService)
    {
        _veterinarianService = veterinarianService;
        Title = "Veterinarios";
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
            var vets = await _veterinarianService.GetAllAsync();
            Veterinarians.Clear();
            foreach (var vet in vets)
                Veterinarians.Add(vet);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudieron cargar los veterinarios: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
