using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class ClinicsViewModel : BaseViewModel
{
    private readonly IClinicApiService _clinicService;

    public ObservableCollection<ClinicDto> Clinics { get; } = new();

    public ClinicsViewModel(IClinicApiService clinicService)
    {
        _clinicService = clinicService;
        Title = "Clínicas";
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
            var clinics = await _clinicService.GetAllAsync();
            Clinics.Clear();
            foreach (var clinic in clinics)
                Clinics.Add(clinic);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudieron cargar las clínicas: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
