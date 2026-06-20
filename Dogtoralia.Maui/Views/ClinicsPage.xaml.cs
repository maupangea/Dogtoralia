using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Views;

public partial class ClinicsPage : ContentPage
{
    private readonly ClinicsViewModel _viewModel;

    public ClinicsPage(ClinicsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Clinics.Count == 0)
            await _viewModel.LoadAsync();
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: ClinicDto clinic })
            await Shell.Current.GoToAsync($"{nameof(ClinicDetailPage)}?id={clinic.Id}");
    }
}
