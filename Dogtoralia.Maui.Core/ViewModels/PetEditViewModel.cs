using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.ViewModels;

public partial class PetEditViewModel : BaseViewModel
{
    private readonly IPetApiService _petService;
    private readonly IPetOwnerApiService _ownerService;

    public static IReadOnlyList<string> SpeciesOptions { get; } =
        new[] { "Perro", "Gato", "Ave", "Conejo", "Hámster", "Otro" };

    public ObservableCollection<PetOwnerDto> Owners { get; } = new();

    [ObservableProperty]
    private int _petId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _selectedSpecies;

    [ObservableProperty]
    private string _breed = string.Empty;

    [ObservableProperty]
    private DateTime _dateOfBirth = new(2020, 1, 1);

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private PetOwnerDto? _selectedOwner;

    public bool IsEdit => PetId > 0;

    public event EventHandler? Saved;

    public PetEditViewModel(IPetApiService petService, IPetOwnerApiService ownerService)
    {
        _petService = petService;
        _ownerService = ownerService;
        Title = "Nueva mascota";
    }

    [RelayCommand]
    public async Task LoadAsync(int petId)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var owners = await _ownerService.GetAllAsync();
            Owners.Clear();
            foreach (var owner in owners)
                Owners.Add(owner);

            PetId = petId;

            if (petId > 0)
            {
                Title = "Editar mascota";
                var pet = await _petService.GetByIdAsync(petId);
                if (pet is not null)
                {
                    Name = pet.Name;
                    SelectedSpecies = pet.Species;
                    Breed = pet.Breed;
                    DateOfBirth = pet.DateOfBirth;
                    Notes = pet.Notes;
                    SelectedOwner = Owners.FirstOrDefault(o => o.Id == pet.PetOwnerId);
                }
            }
            else
            {
                Title = "Nueva mascota";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo cargar el formulario: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "El nombre es obligatorio.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedSpecies))
        {
            ErrorMessage = "La especie es obligatoria.";
            return;
        }

        if (SelectedOwner is null)
        {
            ErrorMessage = "Debe seleccionar un propietario.";
            return;
        }

        var dto = new PetWriteDto
        {
            Name = Name.Trim(),
            Species = SelectedSpecies,
            Breed = Breed?.Trim() ?? string.Empty,
            DateOfBirth = DateOfBirth,
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
            PetOwnerId = SelectedOwner.Id
        };

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            if (IsEdit)
                await _petService.UpdateAsync(PetId, dto);
            else
                await _petService.CreateAsync(dto);

            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo guardar la mascota: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
