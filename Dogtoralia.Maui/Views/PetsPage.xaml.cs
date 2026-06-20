using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Views;

public partial class PetsPage : ContentPage
{
    private readonly PetsViewModel _viewModel;

    public PetsPage(PetsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PetEditPage));
    }

    private async void OnViewClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: PetDto pet })
            await Shell.Current.GoToAsync($"{nameof(PetDetailPage)}?id={pet.Id}");
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: PetDto pet })
            await Shell.Current.GoToAsync($"{nameof(PetEditPage)}?id={pet.Id}");
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: PetDto pet })
            return;

        var confirmed = await DisplayAlertAsync("Eliminar mascota",
            $"¿Deseas eliminar a {pet.Name}?", "Eliminar", "Cancelar");
        if (confirmed)
            await _viewModel.DeleteCommand.ExecuteAsync(pet);
    }
}
