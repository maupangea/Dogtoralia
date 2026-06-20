using Dogtoralia.Maui.Core.ViewModels;

namespace Dogtoralia.Maui.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class VeterinarianDetailPage : ContentPage
{
    private readonly VeterinarianDetailViewModel _viewModel;

    public string ItemId { get; set; } = string.Empty;

    public VeterinarianDetailPage(VeterinarianDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (int.TryParse(ItemId, out var id) && _viewModel.Veterinarian is null)
            await _viewModel.LoadAsync(id);
    }
}
