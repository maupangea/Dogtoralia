using Dogtoralia.Maui.Core.ViewModels;

namespace Dogtoralia.Maui.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class PetDetailPage : ContentPage
{
    private readonly PetDetailViewModel _viewModel;

    public string ItemId { get; set; } = string.Empty;

    public PetDetailPage(PetDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (int.TryParse(ItemId, out var id) && _viewModel.Pet is null)
            await _viewModel.LoadAsync(id);
    }
}
