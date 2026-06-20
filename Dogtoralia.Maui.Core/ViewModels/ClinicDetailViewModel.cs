using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class ClinicDetailViewModel : BaseViewModel
{
    private readonly IClinicApiService _clinicService;

    [ObservableProperty]
    private ClinicDto? _clinic;

    public ClinicDetailViewModel(IClinicApiService clinicService)
    {
        _clinicService = clinicService;
        Title = "Detalle de clínica";
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
            Clinic = await _clinicService.GetByIdAsync(id);
            if (Clinic is not null)
                Title = Clinic.Name;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo cargar la clínica: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
