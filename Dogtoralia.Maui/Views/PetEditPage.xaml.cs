using Dogtoralia.Maui.Core.ViewModels;

namespace Dogtoralia.Maui.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class PetEditPage : ContentPage
{
    private readonly PetEditViewModel _viewModel;
    private bool _loaded;

    public string ItemId { get; set; } = string.Empty;

    public PetEditPage(PetEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.Saved += OnSaved;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loaded)
            return;

        _loaded = true;
        int.TryParse(ItemId, out var id);
        await _viewModel.LoadAsync(id);
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
