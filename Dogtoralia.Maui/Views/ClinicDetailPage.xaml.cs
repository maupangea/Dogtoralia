using Dogtoralia.Maui.Core.ViewModels;

namespace Dogtoralia.Maui.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class ClinicDetailPage : ContentPage
{
    private readonly ClinicDetailViewModel _viewModel;

    public string ItemId { get; set; } = string.Empty;

    public ClinicDetailPage(ClinicDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (int.TryParse(ItemId, out var id) && _viewModel.Clinic is null)
            await _viewModel.LoadAsync(id);
    }
}
