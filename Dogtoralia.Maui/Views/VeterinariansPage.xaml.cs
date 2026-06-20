using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Views;

public partial class VeterinariansPage : ContentPage
{
    private readonly VeterinariansViewModel _viewModel;

    public VeterinariansPage(VeterinariansViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Veterinarians.Count == 0)
            await _viewModel.LoadAsync();
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: VeterinarianDto vet })
            await Shell.Current.GoToAsync($"{nameof(VeterinarianDetailPage)}?id={vet.Id}");
    }
}
